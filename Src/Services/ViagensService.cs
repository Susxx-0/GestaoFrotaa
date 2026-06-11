using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class ViagensService
    {
        private readonly AppDbContext _context;
        private readonly NotificacoesService _notificacaoService;

        public ViagensService(AppDbContext context, NotificacoesService notificacaoService) 
        {
            _context = context;
            _notificacaoService = notificacaoService;
        }

        // INICIAR VIAGEM
        public async Task<Viagem> IniciarViagemAsync(int veiculoId, string condutorPrincipalId, string? condutorSecundarioId = null)
        {
            var veiculoOcupado = await _context.Viagens
                .AnyAsync(v => v.VeiculoId == veiculoId && v.EstaAtiva);

            if (veiculoOcupado)
            {
                throw new Exception("Este veículo já se encontra em uso por outro condutor.");
            }

            var novaViagem = new Viagem
            {
                VeiculoId = veiculoId,
                CondutorPrincipalId = condutorPrincipalId,
                CondutorSecundarioId = condutorSecundarioId,
                DataInicio = DateTime.Now,
                DataLimitePrevista = DateTime.Now.AddHours(4),
                EstaAtiva = true
            };

            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo != null) veiculo.Estado = "Em uso";

            _context.Viagens.Add(novaViagem);
            await _context.SaveChangesAsync();

            return novaViagem;
        }

        // FINALIZAR VIAGEM
        public async Task FinalizarViagemAsync(int viagemId, string? observacoesEntrega)
        {
            var viagem = await _context.Viagens.FindAsync(viagemId);
            if (viagem == null || !viagem.EstaAtiva)
            {
                throw new Exception("Viagem não encontrada ou já finalizada.");
            }

            viagem.DataFim = DateTime.Now;
            viagem.ObservacoesEntrega = observacoesEntrega;
            viagem.EstaAtiva = false;

            var veiculo = await _context.Veiculos.FindAsync(viagem.VeiculoId);
            if (veiculo != null)
            {
                veiculo.Estado = "Disponível";

                // Se o condutor escreveu alguma nota/barulho ao entregar o veículo
                if (!string.IsNullOrWhiteSpace(observacoesEntrega))
                {
                    string alertaMensagem = $"O condutor reportou notas na entrega: {observacoesEntrega}";

                
                    // 1. Notifica o Técnico Responsável por este carro específico (se houver, senão escala para Admin)
                    await _notificacaoService.EnviarNotificacaoAsync(
                        mensagem: alertaMensagem,
                        grau: "TecnicoResponsavel",
                        veiculoId: veiculo.Id
                    );

                    // 2. Notifica também os Administradores de forma geral na plataforma
                    await _notificacaoService.EnviarNotificacaoAsync(
                        mensagem: $"Alerta de Manutenção no veículo {veiculo.Matricula}: {observacoesEntrega}",
                        grau: "Admin",
                        veiculoId: veiculo.Id
                    );
                }
            }

            await _context.SaveChangesAsync();
        }

        // SOLICITAR PRORROGAÇÃO
        public async Task SolicitarMaisTempoAsync(int viagemId, int horasExtras = 2)
        {
            var viagem = await _context.Viagens.FindAsync(viagemId);

            if (viagem == null || !viagem.EstaAtiva)
            {
                throw new Exception("Viagem não encontrada ou já se encontra finalizada.");
            }

            viagem.DataLimitePrevista = viagem.DataLimitePrevista.AddHours(horasExtras);
            viagem.PediuProrrogacao = true;

            await _context.SaveChangesAsync();
        }

        // VERIFICAÇÃO AUTOMÁTICA DE ATRASOS
        public async Task VerificarEAlertarAtrasosAsync()
        {
            var agora = DateTime.Now;

            var viajesAtrasadas = await _context.Viagens
                .Where(v => v.EstaAtiva && agora > v.DataLimitePrevista)
                .ToListAsync();

            foreach (var viagem in viajesAtrasadas)
            {
                var notificacaoExistente = await _context.Notificacoes
                    .AnyAsync(n => n.DestinatarioId == viagem.CondutorPrincipalId && !n.Lida && n.Mensagem.Contains("Excedeu o tempo limite"));

                if (!notificacaoExistente)
                {
                    var veiculo = await _context.Veiculos.FindAsync(viagem.VeiculoId);
                    string matricula = veiculo?.Matricula ?? "Desconhecido";

                    var notificacao = new Notificacao
                    {
                        Mensagem = $"Excedeu o tempo limite de utilização do veículo ({matricula}). Se necessitar de mais tempo, solicite uma prorrogação.",
                        DestinatarioId = viagem.CondutorPrincipalId,
                        Grau = "Aviso",
                        VeiculoId = viagem.VeiculoId,
                        DataCriacao = agora,
                        Lida = false
                    };

                    _context.Notificacoes.Add(notificacao);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}