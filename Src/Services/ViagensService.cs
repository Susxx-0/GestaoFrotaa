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

        // INICIAR VIAGEM ATUALIZADO (Com KM Iniciais Automáticos)
        public async Task<Viagem> IniciarViagemAsync(int veiculoId, string condutorPrincipalId, string? condutorSecundarioId = null)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null)
            {
                throw new Exception("O veículo selecionado não existe no sistema.");
            }

            if (!veiculo.EstaAtivo)
            {
                throw new Exception("Não é possível iniciar viagem: este veículo encontra-se arquivado/inativo.");
            }

            var veiculoOcupado = await _context.Viagens
                .AnyAsync(v => v.VeiculoId == veiculoId && v.EstaAtiva);

            if (veiculoOcupado)
            {
                throw new Exception("Este veículo já se encontra em uso por outro condutor.");
            }

            // Procura os KM Finais da última viagem deste carro para servir de KM Iniciais
            // Se for a primeira viagem de sempre, assume 0 ou os KM base do veículo
            int kmIniciaisDoCarro = await _context.Viagens
                .Where(v => v.VeiculoId == veiculoId && v.KmFinais != null)
                .OrderByDescending(v => v.DataFim)
                .Select(v => v.KmFinais.Value)
                .FirstOrDefaultAsync();

            var novaViagem = new Viagem
            {
                VeiculoId = veiculoId,
                CondutorPrincipalId = condutorPrincipalId,
                CondutorSecundarioId = condutorSecundarioId,
                DataInicio = DateTime.Now,
                DataLimitePrevista = DateTime.Now.AddHours(4),
                EstaAtiva = true,
                KmIniciais = kmIniciaisDoCarro // <--- GRAVA OS KM INICIAIS AUTOMATICAMENTE
            };

            veiculo.Estado = "Em uso";

            _context.Viagens.Add(novaViagem);
            await _context.SaveChangesAsync();

            return novaViagem;
        }

        // FINALIZAR VIAGEM ATUALIZADO (A receber e a gravar os KM Finais)
        public async Task FinalizarViagemAsync(int viagemId, int kmFinais, string? observacoesEntrega)
        {
            var viagem = await _context.Viagens.FindAsync(viagemId);
            if (viagem == null || !viagem.EstaAtiva)
            {
                throw new Exception("Viagem não encontrada ou já finalizada.");
            }

            // Validação de segurança: os KM finais não podem ser menores que os iniciais
            if (kmFinais < viagem.KmIniciais)
            {
                throw new Exception($"Os quilómetros finais ({kmFinais}) não podem ser inferiores aos quilómetros iniciais ({viagem.KmIniciais}).");
            }

            viagem.DataFim = DateTime.Now;
            viagem.ObservacoesEntrega = observacoesEntrega;
            viagem.KmFinais = kmFinais; // <--- AGORA GRAVA OS KM FINAIS NO BANCO!
            viagem.EstaAtiva = false;

            var veiculo = await _context.Veiculos.FindAsync(viagem.VeiculoId);
            if (veiculo != null)
            {
                veiculo.Estado = "Disponível";

                if (!string.IsNullOrWhiteSpace(observacoesEntrega))
                {
                    string alertaMensagem = $"O condutor reportou notas na entrega: {observacoesEntrega}";

                    await _notificacaoService.EnviarNotificacaoAsync(
                        mensagem: alertaMensagem,
                        grau: "TecnicoResponsavel",
                        veiculoId: veiculo.Id
                    );

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


        // HISTÓRICO DE VIAGENS DE UM VEÍCULO
        public async Task<IEnumerable<object>> ObterHistoricoViagensAsync(int veiculoId)
        {
            var viagens = await _context.Viagens
                .Where(v => v.VeiculoId == veiculoId)
                .OrderByDescending(v => v.DataInicio)
                .ToListAsync();

            var historico = new List<object>();

            foreach (var viagem in viagens)
            {
                // Se a viagem ainda estiver ativa, os KM Finais na base de dados podem ser null
                int kmFinais = viagem.KmFinais ?? viagem.KmIniciais;
                int kmsPercorridos = kmFinais - viagem.KmIniciais;

                // Calcula a duração em horas. Se a viagem estiver ativa, calcula até à hora atual (DateTime.Now)
                DateTime fimParaCalculo = viagem.DataFim ?? DateTime.Now;
                double tempoHoras = (fimParaCalculo - viagem.DataInicio).TotalHours;

                // Blindagem matemática contra divisão por zero (evita o crash fatal do JSON)
                double velocidadeMedia = tempoHoras > 0.01 ? (kmsPercorridos / tempoHoras) : 0.0;

                historico.Add(new
                {
                    viagem.Id,
                    viagem.CondutorPrincipalId,
                    viagem.CondutorSecundarioId,
                    viagem.DataInicio,
                    viagem.DataFim,
                    Status = viagem.EstaAtiva ? "Em Curso" : "Finalizada",
                    KmIniciais = viagem.KmIniciais,
                    KmFinais = viagem.KmFinais,
                    KmsPercorridos = kmsPercorridos,
                    DuracaoHoras = Math.Round(tempoHoras, 1),
                    VelocidadeMediaKmH = Math.Round(velocidadeMedia, 1),
                    viagem.ObservacoesEntrega
                });
            }

            return historico;
        }
    }
}