using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using System;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class NotificacaoService
    {
        private readonly AppDbContext _context;

        public NotificacaoService(AppDbContext context)
        {
            _context = context;
        }

        // Cria uma notificação no sistema para um utilizador ou grupo específico
        public async Task CriarNotificacaoAsync(int veiculoId, string mensagem, string destinatarioId, string grau = "Aviso")
        {
            var notificacao = new Notificacao
            {
                VeiculoId = veiculoId,
                Mensagem = mensagem,
                DestinatarioId = destinatarioId,
                Grau = grau,
                DataCriacao = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }
    }
}