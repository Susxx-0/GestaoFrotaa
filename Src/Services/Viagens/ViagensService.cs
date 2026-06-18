using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Viagens
{
    public class ViagensService
    {
        private readonly AppDbContext _context;

        public ViagensService(AppDbContext context)
        {
            _context = context;
        }

        // INICIAR VIAGEM (versão simples)
        public async Task<Viagem> IniciarViagemAsync(int veiculoId, string condutorId)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null || !veiculo.EstaAtivo)
                throw new Exception("Veículo não encontrado ou inativo.");

            if (veiculo.Estado == "Em Manutenção")
                throw new Exception("Veículo em manutenção não pode iniciar viagem.");

            if (veiculo.Estado == "Em uso")
                throw new Exception("Este veículo já possui uma viagem ativa.");

            var nova = new Viagem
            {
                VeiculoId = veiculoId,
                CondutorPrincipalId = condutorId,
                DataInicio = DateTime.Now,
                DataLimitePrevista = DateTime.Now.AddHours(4),
                EstaAtiva = true,
                KmIniciais = veiculo.KmAtual
            };

            veiculo.Estado = "Em uso";

            _context.Viagens.Add(nova);
            await _context.SaveChangesAsync();

            return nova;
        }

        // INICIAR VIAGEM (com condutor secundário)
        public async Task<Viagem> IniciarViagemAsync(int veiculoId, string condutorPrincipalId, string? condutorSecundarioId)
        {
            var viagem = await IniciarViagemAsync(veiculoId, condutorPrincipalId);
            viagem.CondutorSecundarioId = condutorSecundarioId;

            await _context.SaveChangesAsync();
            return viagem;
        }

        // FINALIZAR VIAGEM
        public async Task FinalizarViagemAsync(int viagemId, int kmFinais, string? observacoes)
        {
            var viagem = await _context.Viagens
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(v => v.Id == viagemId);

            if (viagem == null || !viagem.EstaAtiva)
                throw new Exception("Viagem não encontrada ou já finalizada.");

            if (kmFinais < viagem.KmIniciais)
                throw new Exception("KM finais inválidos.");

            viagem.KmFinais = kmFinais;
            viagem.DataFim = DateTime.Now;
            viagem.ObservacoesEntrega = observacoes;
            viagem.EstaAtiva = false;

            viagem.Veiculo.Estado = "Disponível";
            viagem.Veiculo.KmAtual = kmFinais;

            await _context.SaveChangesAsync();
        }

        // HISTÓRICO DE VIAGENS
        public async Task<IEnumerable<object>> ObterHistoricoViagensAsync(int veiculoId)
        {
            var viagens = await _context.Viagens
                .Where(v => v.VeiculoId == veiculoId)
                .OrderByDescending(v => v.DataInicio)
                .ToListAsync();

            return viagens.Select(v => new
            {
                v.Id,
                v.DataInicio,
                v.DataFim,
                Status = v.EstaAtiva ? "Em Curso" : "Finalizada",
                KmPercorridos = (v.KmFinais ?? v.KmIniciais) - v.KmIniciais,
                Observacoes = v.ObservacoesEntrega
            });
        }

        // VERIFICAR ATRASOS
        public async Task VerificarEAlertarAtrasosAsync()
        {
            var hoje = DateTime.Now;

            var atrasadas = await _context.Viagens
                .Where(v => v.EstaAtiva && hoje > v.DataLimitePrevista)
                .ToListAsync();

            foreach (var v in atrasadas)
            {
                bool jaExiste = await _context.Notificacoes
                    .AnyAsync(n => n.VeiculoId == v.VeiculoId && !n.Lida);

                if (!jaExiste)
                {
                    _context.Notificacoes.Add(new Notificacao
                    {
                        Mensagem = $"A viagem do veículo ID {v.VeiculoId} ultrapassou o tempo limite.",
                        Grau = "Aviso",
                        DataCriacao = DateTime.Now,
                        VeiculoId = v.VeiculoId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        // PRORROGAÇÃO DE TEMPO
        public async Task<bool> SolicitarMaisTempoAsync(int viagemId, int horas)
        {
            var viagem = await _context.Viagens.FindAsync(viagemId);
            if (viagem == null || !viagem.EstaAtiva)
                return false;

            viagem.DataLimitePrevista = viagem.DataLimitePrevista.AddHours(horas);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
