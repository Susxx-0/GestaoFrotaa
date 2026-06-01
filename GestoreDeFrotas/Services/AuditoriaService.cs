using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Services
{
    public class AuditoriaService
    {
        private readonly AppDbContext _context;

        public AuditoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistarLogAsync(string? utilizador, string metodo, string rota, string descricao, int statusCode)
        {
            var log = new LogSistema
            {
                Utilizador = utilizador ?? "Anónimo",
                MetodoHttp = metodo,
                Rota = rota,
                Descricao = descricao,
                StatusCode = statusCode
            };

            _context.LogsSistema.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LogSistema>> ObterLogsAsync()
        {
            return await _context.LogsSistema
                .OrderByDescending(l => l.Data)
                .Take(200)
                .ToListAsync();
        }

        public async Task CriarNotificacaoAsync(string titulo, string mensagem, string tipo = "Info")
        {
            var notif = new Notificacao
            {
                Titulo = titulo,
                Mensagem = mensagem,
                Tipo = tipo
            };

            _context.Notificacoes.Add(notif);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notificacao>> ObterNotificacoesAtivasAsync()
        {
            return await _context.Notificacoes
                .Where(n => !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task MarcarComoLidaAsync(int id)
        {
            var notif = await _context.Notificacoes.FindAsync(id);
            if (notif != null)
            {
                notif.Lida = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}