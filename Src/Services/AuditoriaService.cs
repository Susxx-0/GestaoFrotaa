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

            // ---------------------------------------------------------
            // REGISTAR LOG NA BASE DE DADOS
            // ---------------------------------------------------------
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

            // ---------------------------------------------------------
            // OBTER ÚLTIMOS 200 LOGS
            // ---------------------------------------------------------
            public async Task<IEnumerable<LogSistema>> ObterLogsAsync()
            {
                return await _context.LogsSistema
                    .OrderByDescending(l => l.Data)
                    .Take(200)
                    .ToListAsync();
            }

            // ---------------------------------------------------------
            // NOTIFICAÇÕES ATIVAS
            // ---------------------------------------------------------
            public async Task<IEnumerable<Notificacao>> ObterNotificacoesAtivasAsync()
            {
                return await _context.Notificacoes
                    .Where(n => !n.Lida)
                    .OrderByDescending(n => n.DataCriacao)
                    .ToListAsync();
            }

            // ---------------------------------------------------------
            // MARCAR NOTIFICAÇÃO COMO LIDA
            // ---------------------------------------------------------
            public async Task MarcarComoLidaAsync(int id)
            {
                var notificacao = await _context.Notificacoes.FindAsync(id);

                if (notificacao != null)
                {
                    notificacao.Lida = true;
                    await _context.SaveChangesAsync();
                }
            }

            // ---------------------------------------------------------
            // CRIAR NOTIFICAÇÃO (SEM TÍTULO)
            // ---------------------------------------------------------
            public async Task CriarNotificacaoAsync(string titulo, string mensagem, string grau)
            {
                // Se a mensagem vier vazia, usa o título como fallback
                var mensagemFinal = string.IsNullOrWhiteSpace(mensagem)
                    ? (string.IsNullOrWhiteSpace(titulo) ? "Notificação do sistema" : titulo)
                    : mensagem;

                var notificacao = new Notificacao
                {
                    Mensagem = mensagemFinal,
                    Grau = string.IsNullOrWhiteSpace(grau) ? "Info" : grau,
                    DataCriacao = DateTime.Now,
                    Lida = false,
                    DestinatarioId = "Sistema"
                };

                _context.Notificacoes.Add(notificacao);
                await _context.SaveChangesAsync();
            }
        }
    }
