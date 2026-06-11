using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using GestoreDeFrotas. Services;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services
{
    public class NotificacoesService
    {
        private readonly AppDbContext _context;

        public NotificacoesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task EnviarNotificacaoAsync(string mensagem, string grau, int? veiculoId = null, string? destinatarioEspecificoId = null)
        {
            var novaNotificacao = new Notificacao
            {
                DataCriacao = DateTime.Now,
                Grau = grau, // Ex: "Geral", "Admin", "Tecnico", "TecnicoResponsavel"
                VeiculoId = veiculoId
            };

            // Regra 1: Se for para um utilizador específico (ex: um condutor que fez asneira)
            if (grau == "Especifico" && !string.IsNullOrEmpty(destinatarioEspecificoId))
            {
                novaNotificacao.DestinatarioId = destinatarioEspecificoId;
                novaNotificacao.Mensagem = mensagem; // Garante que mapeia com a tua propriedade de texto (Mensagem/Descricao)
            }

            // Regra 2: Se for direcionado ao Técnico Encarregue daquele carro específico
            else if (grau == "TecnicoResponsavel" && veiculoId.HasValue)
            {
                var veiculo = await _context.Veiculos
                    .FirstOrDefaultAsync(v => v.Id == veiculoId.Value);

                if (veiculo != null && !string.IsNullOrEmpty(veiculo.TecnicoResponsavelId))
                {
                    novaNotificacao.DestinatarioId = veiculo.TecnicoResponsavelId;
                    novaNotificacao.Mensagem = $"[Aviso Veículo {veiculo.Matricula}] " + mensagem;
                }
                else
                {
                    // Se o carro não tiver técnico associado, escala automaticamente para os Admins Gerais
                    novaNotificacao.Grau = "Admin";
                    novaNotificacao.Mensagem = $"[Sem Técnico - Carro {veiculo?.Matricula}] " + mensagem;
                }
            }

            // Regra 3: Para Admins, Técnicos Gerais ou Avisos Gerais
            else
            {
                novaNotificacao.DestinatarioId = null; // Fica aberto para o perfil correspondente ler
                novaNotificacao.Mensagem = mensagem;
            }

            _context.Notificacoes.Add(novaNotificacao);
            await _context.SaveChangesAsync();
        }
    }
}