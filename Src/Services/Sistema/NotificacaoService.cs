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

        // ============================
        // 1. OBTER TODAS AS NOTIFICAÇÕES
        // ============================
        public async Task<IEnumerable<Notificacao>> ObterTodasAsync()
        {
            return await _context.Notificacoes
                .OrderByDescending(n => n.Data)
                .AsNoTracking()
                .ToListAsync();
        }

        // ============================
        // 2. CRIAR NOTIFICAÇÃO
        // ============================
        public async Task CriarAsync(string titulo, string mensagem, int? veiculoId = null)
        {
            var notificacao = new Notificacao
            {
                Titulo = titulo,
                Mensagem = mensagem,
                VeiculoId = veiculoId,
                Data = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }

        // ============================
        // 3. MARCAR COMO LIDA
        // ============================
        public async Task<bool> MarcarComoLidaAsync(int id)
        {
            var notif = await _context.Notificacoes.FindAsync(id);
            if (notif == null)
                return false;

            notif.Lida = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // 4. NOTIFICAÇÕES DE IPO
        // ============================
        public async Task GerarNotificacoesIPOAsync()
        {
            var hoje = DateTime.Now.Date;
            var limite = hoje.AddDays(30);

            var veiculos = await _context.Veiculos
                .Where(v => v.DataProximaIpo != null && v.DataProximaIpo <= limite)
                .ToListAsync();

            foreach (var v in veiculos)
            {
                string msg = v.DataProximaIpo < hoje
                    ? "IPO expirada — veículo não pode circular."
                    : $"IPO expira em {(v.DataProximaIpo.Value - hoje).Days} dias.";

                await CriarAsync(
                    "Alerta de IPO",
                    $"{v.Marca} {v.Modelo} ({v.Matricula}): {msg}",
                    v.Id
                );
            }
        }

        // ============================
        // 5. NOTIFICAÇÕES DE SEGURO
        // ============================
        public async Task GerarNotificacoesSeguroAsync()
        {
            var hoje = DateTime.Now.Date;
            var limite = hoje.AddDays(30);

            var veiculos = await _context.Veiculos
                .Where(v => v.DataSeguro != null && v.DataSeguro <= limite)
                .ToListAsync();

            foreach (var v in veiculos)
            {
                string msg = v.DataSeguro < hoje
                    ? "Seguro expirado — veículo não pode circular."
                    : $"Seguro expira em {(v.DataSeguro.Value - hoje).Days} dias.";

                await CriarAsync(
                    "Alerta de Seguro",
                    $"{v.Marca} {v.Modelo} ({v.Matricula}): {msg}",
                    v.Id
                );
            }
        }

        // ============================
        // 6. NOTIFICAÇÕES DE DOCUMENTOS
        // ============================
        public async Task GerarNotificacoesDocumentosAsync()
        {
            var hoje = DateTime.Now.Date;
            var limite = hoje.AddDays(30);

            var docs = await _context.DocumentosVeiculos
                .Include(d => d.Veiculo)
                .Where(d => d.DataValidade != null && d.DataValidade <= limite)
                .ToListAsync();

            foreach (var d in docs)
            {
                string msg = d.DataValidade < hoje
                    ? "Documento expirado."
                    : $"Documento expira em {(d.DataValidade.Value - hoje).Days} dias.";

                await CriarAsync(
                    "Alerta de Documento",
                    $"{d.Veiculo.Marca} {d.Veiculo.Modelo} ({d.Veiculo.Matricula}): {msg}",
                    d.VeiculoId
                );
            }
        }

        // ============================
        // 7. NOTIFICAÇÕES DE MANUTENÇÃO ATRASADA
        // ============================
        public async Task GerarNotificacoesManutencaoAtrasadaAsync()
        {
            var hoje = DateTime.Now.Date;

            var atrasadas = await _context.RegistosManutencao
                .Include(m => m.Veiculo)
                .Where(m => m.DataPrevista != null && m.DataPrevista < hoje && m.DataConclusao == null)
                .ToListAsync();

            foreach (var m in atrasadas)
            {
                await CriarAsync(
                    "Manutenção Atrasada",
                    $"{m.Veiculo.Marca} {m.Veiculo.Modelo} ({m.Veiculo.Matricula}): manutenção atrasada há {(hoje - m.DataPrevista.Value).Days} dias.",
                    m.VeiculoId
                );
            }
        }

        // ============================
        // 8. EXECUTAR TODAS AS NOTIFICAÇÕES
        // ============================
        public async Task GerarTodasAsync()
        {
            await GerarNotificacoesIPOAsync();
            await GerarNotificacoesSeguroAsync();
            await GerarNotificacoesDocumentosAsync();
            await GerarNotificacoesManutencaoAtrasadaAsync();
        }
    }
}
