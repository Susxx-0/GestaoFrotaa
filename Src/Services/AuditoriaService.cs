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

        public AuditoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistarLogAsync(string utilizador, string metodo, string rota, string descricao, int statusCode)
        {
           
            var utilizadorLimpo = string.IsNullOrWhiteSpace(utilizador) ? "Anónimo" : utilizador;
            var metodoLimpo = string.IsNullOrWhiteSpace(metodo) ? "UNKNOWN" : (metodo.Length > 10 ? metodo.Substring(0, 10) : metodo);
            var rotaLimpa = string.IsNullOrWhiteSpace(rota) ? "/" : (rota.Length > 250 ? rota.Substring(0, 250) : rota);
            var descricaoLimpa = string.IsNullOrWhiteSpace(descricao) ? "Sem descrição" : descricao;

            var log = new LogSistema
            {
                Utilizador = utilizadorLimpo,
                MetodoHttp = metodoLimpo,
                Rota = rotaLimpa,
                Descricao = descricaoLimpa,
                StatusCode = statusCode,
                Data = DateTime.Now
            };

            try
            {
                _context.LogsSistema.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               
                System.Diagnostics.Debug.WriteLine($"Falha crítica ao persistir log de auditoria: {ex.Message}");
            }
        }

        public async Task<IEnumerable<LogSistema>> ObterLogsAsync()
        {
            return await _context.LogsSistema
                .OrderByDescending(l => l.Data)
                .Take(200)
                .ToListAsync();
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
            var notificacao = await _context.Notificacoes.FindAsync(id);

            if (notificacao != null)
            {
                notificacao.Lida = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CriarNotificacaoAsync(string mensagem, string destinatarioId, string grau)
        {
            var notificacao = new Notificacao
            {
               
                DataCriacao = DateTime.Now,
                Lida = false,

              
                DestinatarioId = string.IsNullOrWhiteSpace(destinatarioId) ? "Sistema" : destinatarioId
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }
    }
}