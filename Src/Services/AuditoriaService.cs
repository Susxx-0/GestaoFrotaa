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

        // Registo de Logs do Sistema
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

        // Obter os últimos 200 logs
        public async Task<IEnumerable<LogSistema>> ObterLogsAsync()
        {
            return await _context.LogsSistema
                .OrderByDescending(l => l.Data)
                .Take(200)
                .ToListAsync();
        }

        // CORREGIDO: Ajustado para as propriedades reais do modelo Notificacao
        public async Task<IEnumerable<Notificacao>> ObterNotificacoesAtivasAsync()
        {
            return await _context.Notificacoes
                .Where(n => !n.Lida)
                .OrderByDescending(n => n.DataCriacao) // Mudado de .Data para .DataCriacao
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

        // CORREGIDO: Ajustado para mapear com o novo modelo Notificacao
        public async Task CriarNotificacaoAsync(string mensagem, string destinatarioId, string grau)
        {
            var notificacao = new Notificacao
            {
                Mensagem = mensagem,
                DestinatarioId = destinatarioId, // Quem vai receber (Admin, Técnico, ou ID do User)
                Grau = grau, // "Info", "Aviso", "Critico"
                DataCriacao = DateTime.Now,
                Lida = false
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();
        }
    }
}