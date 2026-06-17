using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<Viagem> IniciarViagemAsync(int veiculoId, string condutorPrincipalId, string? condutorSecundarioId = null)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null || !veiculo.EstaAtivo) throw new Exception("O veículo selecionado não existe ou está inativo.");

            
            if (veiculo.Estado == "Em Manutenção") throw new Exception("Bloqueio de Segurança: Este veículo encontra-se em manutenção e não pode iniciar viagem.");
            if (veiculo.Estado == "Em uso") throw new Exception("Este veículo já está associado a uma viagem ativa.");

            int kmIniciaisDoCarro = await _context.Viagens
                .Where(v => v.VeiculoId == veiculoId && v.KmFinais != null)
                .OrderByDescending(v => v.DataFim)
                .Select(v => v.KmFinais!.Value)
                .FirstOrDefaultAsync();

            if (kmIniciaisDoCarro == 0) kmIniciaisDoCarro = veiculo.KmAtual;

            var novaViagem = new Viagem
            {
                VeiculoId = veiculoId,
                CondutorPrincipalId = condutorPrincipalId,
                CondutorSecundarioId = condutorSecundarioId,
                DataInicio = DateTime.Now,
                DataLimitePrevista = DateTime.Now.AddHours(4),
                EstaAtiva = true,
                KmIniciais = kmIniciaisDoCarro
            };

            veiculo.Estado = "Em uso";
            _context.Viagens.Add(novaViagem);
            await _context.SaveChangesAsync();
            return novaViagem;
        }

        public async Task FinalizarViagemAsync(int viagemId, int kmFinais, string? observacoesEntrega)
        {
            var viagem = await _context.Viagens.Include(v => v.Veiculo).FirstOrDefaultAsync(v => v.Id == viagemId);
            if (viagem == null || !viagem.EstaAtiva) throw new Exception("Viagem não encontrada ou já concluída.");
            if (kmFinais < viagem.KmIniciais) throw new Exception($"Quilometragem inválida. Menor que os KM Iniciais ({viagem.KmIniciais}).");

            viagem.DataFim = DateTime.Now;
            viagem.ObservacoesEntrega = observacoesEntrega;
            viagem.KmFinais = kmFinais;
            viagem.EstaAtiva = false;

            if (viagem.Veiculo != null)
            {
                viagem.Veiculo.Estado = "Disponível";
                viagem.Veiculo.KmAtual = kmFinais;

              
                if (!string.IsNullOrWhiteSpace(observacoesEntrega))
                {
                    await _notificacaoService.EnviarNotificacaoAsync(
                        $"Aviso de Entrega ({viagem.Veiculo.Matricula}): {observacoesEntrega}",
                        "TecnicoResponsavel",
                        viagem.Veiculo.Id
                    );
                }

                if (viagem.Veiculo.KmAtual >= viagem.Veiculo.ProximaManutencaoKm)
                {
                    await _notificacaoService.EnviarNotificacaoAsync(
                        $"Alerta Crítico: Veículo {viagem.Veiculo.Matricula} ultrapassou o limite de quilómetros para revisão ({viagem.Veiculo.ProximaManutencaoKm} KM).",
                        "Admin",
                        viagem.Veiculo.Id
                    );
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> ObterHistoricoViagensAsync(int veiculoId)
        {
            var viagens = await _context.Viagens.Where(v => v.VeiculoId == veiculoId).OrderByDescending(v => v.DataInicio).ToListAsync();
            return viagens.Select(v => {
                int kmFinais = v.KmFinais ?? v.KmIniciais;
                double horas = ((v.DataFim ?? DateTime.Now) - v.DataInicio).TotalHours;
                return new
                {
                    v.Id,
                    v.DataInicio,
                    v.DataFim,
                    Status = v.EstaAtiva ? "Em Curso" : "Finalizada",
                    KmsPercorridos = kmFinais - v.KmIniciais,
                    DuracaoHoras = Math.Round(horas, 1),
                    VelocidadeMediaKmH = horas > 0.1 ? Math.Round((kmFinais - v.KmIniciais) / horas, 1) : 0,
                    v.ObservacoesEntrega
                };
            });
        }
    }
}