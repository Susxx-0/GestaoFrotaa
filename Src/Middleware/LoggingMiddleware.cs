using GestoreDeFrotas.Services; 
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, AuditoriaService auditoriaService)
        {
            await _next(context);

            var username = (context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.Name)?.Value
                : "Anónimo") ?? "Anónimo";

            var rota = context.Request.Path.ToString();
            var metodo = context.Request.Method;
            var statusCode = context.Response.StatusCode;

            var descricao = $"Ação HTTP {metodo} executada na rota '{rota}' com resposta {statusCode}.";
            await auditoriaService.RegistarLogAsync(username, metodo, rota, descricao, statusCode);

            if (statusCode >= 400)
            {
                await auditoriaService.CriarNotificacaoAsync($"Erro Operacional {statusCode}", $"O utilizador '{username}' falhou a operação {metodo} em {rota}.", "Error");
            }
        }
    }
}