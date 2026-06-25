using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Notificacoes
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. Obter notificações de um utilizador
        // ============================================================
        public async Task<IEnumerable<Notificacao>> ObterPorUtilizadorAsync(int userId)
        {
            return await _context.Notificacoes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.DataCriacao)
                .AsNoTracking()
                .ToListAsync();
        }

        // ============================================================
        // 2. Criar notificação
        // ============================================================
        public async Task CriarAsync(
            string titulo,
            string mensagem,
            int? userId = null,
            int? veiculoId = null,
            string tipo = "Info",
            string grau = "Info")
        {
            var notif = new Notificacao
            {
                Titulo = titulo,
                Mensagem = mensagem,
                UserId = userId,
                VeiculoId = veiculoId,
                Tipo = tipo,
                Grau = grau,
                DataCriacao = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(notif);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // 3. Marcar como lida
        // ============================================================
        public async Task<bool> MarcarComoLidaAsync(int id)
        {
            var notif = await _context.Notificacoes.FindAsync(id);
            if (notif == null)
                return false;

            notif.Lida = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // 4. Notificações de carta (para o próprio utilizador)
        // ============================================================
        public async Task GerarNotificacoesCartaAsync()
        {
            var hoje = DateTime.Today;
            var limite = hoje.AddDays(30);

            var users = await _context.Users
                .Where(u => u.CartaConducaoValidade != null &&
                            u.CartaConducaoValidade <= limite)
                .ToListAsync();

            foreach (var u in users)
            {
                var dias = (u.CartaConducaoValidade.Value - hoje).Days;

                await CriarAsync(
                    "Validade da Carta",
                    $"A sua carta expira em {dias} dias.",
                    userId: u.Id,
                    tipo: "CartaExpira",
                    grau: dias <= 5 ? "Urgente" : "Aviso"
                );
            }
        }

        // ============================================================
        // 5. Notificações de IPO (para gestores)
        // ============================================================
        public async Task GerarNotificacoesIPOAsync()
        {
            var hoje = DateTime.Today;
            var limite = hoje.AddDays(30);

            var gestores = await _context.Users
                .Where(u => u.Role == "Admin" || u.Role == "Gestor")
                .ToListAsync();

            var veiculos = await _context.Veiculos
                .Where(v => v.DataProximaIpo != null &&
                            v.DataProximaIpo <= limite)
                .ToListAsync();

            foreach (var v in veiculos)
            {
                var dias = (v.DataProximaIpo.Value - hoje).Days;

                foreach (var g in gestores)
                {
                    await CriarAsync(
                        "IPO a Expirar",
                        $"O veículo {v.Matricula} tem IPO a expirar em {dias} dias.",
                        userId: g.Id,
                        veiculoId: v.Id,
                        tipo: "IPOExpira",
                        grau: dias <= 5 ? "Urgente" : "Aviso"
                    );
                }
            }
        }

        // ============================================================
        // 6. Notificações de Seguro (para gestores)
        // ============================================================
        public async Task GerarNotificacoesSeguroAsync()
        {
            var hoje = DateTime.Today;
            var limite = hoje.AddDays(30);

            var gestores = await _context.Users
                .Where(u => u.Role == "Admin" || u.Role == "Gestor")
                .ToListAsync();

            var veiculos = await _context.Veiculos
                .Where(v => v.DataSeguro != null &&
                            v.DataSeguro <= limite)
                .ToListAsync();

            foreach (var v in veiculos)
            {
                var dias = (v.DataSeguro.Value - hoje).Days;

                foreach (var g in gestores)
                {
                    await CriarAsync(
                        "Seguro a Expirar",
                        $"O seguro do veículo {v.Matricula} expira em {dias} dias.",
                        userId: g.Id,
                        veiculoId: v.Id,
                        tipo: "SeguroExpira",
                        grau: dias <= 5 ? "Urgente" : "Aviso"
                    );
                }
            }
        }

        // ============================================================
        // 7. Notificações de viagens atrasadas 
        // ============================================================
        public async Task GerarNotificacoesViagensAtrasadasAsync()
        {
            var viagens = await _context.Viagens
                .Where(v => v.DataFim == null)
                .ToListAsync();

            foreach (var v in viagens)
            {
                var horas = (DateTime.Now - v.DataInicio).TotalHours;

                if (horas >= 8) // podes mudar este valor
                {
                    var condutor = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == v.CondutorPrincipalId);

                    if (condutor != null)
                    {
                        await CriarAsync(
                            "Viagem Atrasada",
                            $"A viagem iniciada às {v.DataInicio} já ultrapassou o limite de tempo.",
                            userId: condutor.Id,
                            veiculoId: v.VeiculoId,
                            tipo: "ViagemAtrasada",
                            grau: "Urgente"
                        );
                    }
                }
            }
        }

        // ============================================================
        // 8. Executar todas as notificações
        // ============================================================
        public async Task GerarTodasAsync()
        {
            await GerarNotificacoesCartaAsync();
            await GerarNotificacoesIPOAsync();
            await GerarNotificacoesSeguroAsync();
            await GerarNotificacoesViagensAtrasadasAsync();
        }
    }
}
