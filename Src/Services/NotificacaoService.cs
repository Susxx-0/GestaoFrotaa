using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class NotificacoesService
    {
        private readonly AppDbContext _context;
        public NotificacoesService(AppDbContext context) => _context = context;

        public async Task EnviarNotificacaoAsync(string mensagem, string grau, int? veiculoId = null, string destinatarioId = "Admin")
        {
            var notificacao = new Notificacao
            {
                Mensagem = mensagem,
                Grau = grau,
                VeiculoId = veiculoId,
                DestinatarioId = destinatarioId,
                DataCriacao = DateTime.Now,
                Lida = false
            };
            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }

        public async Task VerificarEGerarAlertasAgendadosAsync()
        {
            var hoje = DateTime.Now;

            // Verificar Veículos com Manutenção Prazos ou Quilometragem em Atraso
            var veiculosEmRisco = await _context.Veiculos
                .Where(v => v.EstaAtivo && (v.KmAtual >= (v.UltimaManutencaoKm + 15000) || (v.UltimaManutencaoData != null && hoje >= v.UltimaManutencaoData.Value.AddMonths(12))))
                .ToListAsync();

            foreach (var carro in veiculosEmRisco)
            {
                bool jaNotificado = await _context.Notificacoes
                    .AnyAsync(n => n.VeiculoId == carro.Id && n.Grau == "Admin" && !n.Lida && n.Mensagem.Contains("manutenção atrasada"));

                if (!jaNotificado)
                {
                    await EnviarNotificacaoAsync(
                        $"O veículo {carro.Marca} {carro.Modelo} ({carro.Matricula}) está com a manutenção atrasada! KM Atual: {carro.KmAtual}.",
                        "Admin",
                        carro.Id
                    );
                }
            }

            // Verificar Viagens que ultrapassaram a data limite prevista de entrega
            var viagensAtrasadas = await _context.Viagens
                .Include(v => v.Veiculo)
                .Where(v => v.EstaAtiva && hoje > v.DataLimitePrevista)
                .ToListAsync();

            foreach (var viagem in viagensAtrasadas)
            {
                bool jaNotificadoAtraso = await _context.Notificacoes
                    .AnyAsync(n => n.DestinatarioId == viagem.CondutorPrincipalId && !n.Lida && n.Mensagem.Contains("tempo limite"));

                if (!jaNotificadoAtraso)
                {
                    var matricula = viagem.Veiculo?.Matricula ?? "Desconhecida";
                    await EnviarNotificacaoAsync(
                        $"Excedeu o tempo limite de utilização do veículo ({matricula}). Submeta um pedido de prorrogação ou entregue o carro.",
                        "Aviso",
                        viagem.VeiculoId,
                        viagem.CondutorPrincipalId
                    );
                }
            }
        }
    }
}