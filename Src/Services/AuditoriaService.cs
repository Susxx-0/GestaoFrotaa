using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class AuditoriaService
    {
        private readonly AppDbContext _context;
        public AuditoriaService(AppDbContext context) => _context = context;

        public async Task RegistarLogAsync(string user, string metodo, string rota, string desc, int status)
        {
            _context.LogsSistema.Add(new LogSistema
            {
                Utilizador = user,
                Metodo = metodo,
                MetodoHttp = metodo,
                Rota = rota,
                Descricao = desc,
                StatusCode = status,
                Data = DateTime.Now,
                DataRegisto = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notificacao>> ObterNotificacoesAtivasAsync() =>
            await _context.Notificacoes.Where(n => !n.Lida).ToListAsync();

        public async Task<bool> MarcarComoLidaAsync(int id)
        {
            var n = await _context.Notificacoes.FindAsync(id);
            if (n == null) return false;
            n.Lida = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<LogSistema>> ObterLogsAsync() =>
            await _context.LogsSistema.OrderByDescending(l => l.DataRegisto).ToListAsync();

        // 🔥 MÉTODO EM FALTA — AGORA COMPLETO
        public async Task CriarNotificacaoAsync(string mensagem, string grau = "Aviso", int? veiculoId = null)
        {
            var n = new Notificacao
            {
                Mensagem = mensagem,
                Grau = grau,
                VeiculoId = veiculoId,
                DestinatarioId = "Sistema",
                DataCriacao = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(n);
            await _context.SaveChangesAsync();
        }
    }
}
