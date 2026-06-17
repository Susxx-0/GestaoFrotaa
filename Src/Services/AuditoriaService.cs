using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using System;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class AuditoriaService
    {
        private readonly AppDbContext _context;
        public AuditoriaService(AppDbContext context) => _context = context;

        public async Task RegistarLogAsync(string user, string metodo, string rota, string descricao, int status)
        {
            var log = new LogSistema
            {
                Utilizador = user,
                MetodoHttp = metodo,
                Rota = rota,
                Descricao = descricao,
                StatusCode = status,
                DataRegisto = DateTime.Now
            };
            _context.LogsSistema.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task CriarNotificacaoAsync(string mensagem, string descricao, string grau)
        {
            var notificacao = new Notificacao
            {
                Mensagem = $"{mensagem}: {descricao}",
                Grau = grau,
                DestinatarioId = "Admin",
                DataCriacao = DateTime.Now
            };
            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }
    }
}