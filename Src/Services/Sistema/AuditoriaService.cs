using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Sistema
{
    public class AuditoriaService
    {
        private readonly AppDbContext _context;

        public AuditoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LogSistema>> ObterTodosAsync()
        {
            return await _context.LogsSistema
                .OrderByDescending(l => l.DataRegisto)
                .ToListAsync();
        }

        public async Task<LogSistema?> ObterPorIdAsync(int id)
        {
            return await _context.LogsSistema.FindAsync(id);
        }

        public async Task RegistarLogAsync(
            string utilizador,
            string metodo,
            string rota,
            string descricao,
            int statusCode)
        {
            var log = new LogSistema
            {
                Data = DateTime.Now,
                DataRegisto = DateTime.Now,
                Metodo = metodo,
                MetodoHttp = metodo,
                Rota = rota,
                Descricao = descricao,
                StatusCode = statusCode,
                Utilizador = utilizador ?? "Sistema"
            };

            _context.LogsSistema.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task CriarNotificacaoAsync(string mensagem, string tipo, int? veiculoId = null)
        {
            var notif = new Notificacao
            {
                Mensagem = mensagem,
                Tipo = tipo,
                Grau = "Info",
                DataCriacao = DateTime.Now,
                Lida = false,
                VeiculoId = veiculoId
            };

            _context.Notificacoes.Add(notif);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notificacao>> ObterNotificacoesAtivasAsync()
        {
            return await _context.Notificacoes
                .Where(n => !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task MarcarComoLidaAsync(int id)
        {
            var notif = await _context.Notificacoes.FindAsync(id);
            if (notif == null)
                return;

            notif.Lida = true;
            await _context.SaveChangesAsync();
        }
    }
}
