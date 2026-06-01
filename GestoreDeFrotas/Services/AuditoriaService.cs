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

        // -----------------------------------------
        // 🔹 REGISTAR LOG
        // -----------------------------------------
        public async Task RegistarLogAsync(string utilizador, string metodo, string rota, string descricao, int statusCode)
        {
            var log = new LogSistema
            {
                Utilizador = utilizador ?? "Anónimo",
                MetodoHttp = metodo,
                Rota = rota,
                Descricao = descricao,
                StatusCode = statusCode,
                Data = DateTime.Now
            };

            _context.LogsSistema.Add(log);
            await _context.SaveChangesAsync();
        }

        // -----------------------------------------
        // 🔹 OBTER LOGS
        // -----------------------------------------
        public async Task<IEnumerable<LogSistema>> ObterLogsAsync()
        {
            return await _context.LogsSistema
                .OrderByDescending(l => l.Data)
                .Take(200)
                .ToListAsync();
        }

        // -----------------------------------------
        // 🔹 OBTER NOTIFICAÇÕES ATIVAS
        // -----------------------------------------
        public async Task<IEnumerable<Notificacao>> ObterNotificacoesAtivasAsync()
        {
            return await _context.Notificacoes
                .Where(n => !n.Lida)
                .OrderByDescending(n => n.Data)
                .ToListAsync();
        }

        // -----------------------------------------
        // 🔹 MARCAR NOTIFICAÇÃO COMO LIDA
        // -----------------------------------------
        public async Task MarcarComoLidaAsync(int id)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);

            if (notificacao != null)
            {
                notificacao.Lida = true;
                await _context.SaveChangesAsync();
            }
        }

        // -----------------------------------------
        // 🔹 CRIAR NOTIFICAÇÃO
        // -----------------------------------------
        public async Task CriarNotificacaoAsync(string titulo, string mensagem, string tipo)
        {
            var notificacao = new Notificacao
            {
                Titulo = titulo,
                Mensagem = mensagem,
                Tipo = tipo,
                Data = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }
    }
}
