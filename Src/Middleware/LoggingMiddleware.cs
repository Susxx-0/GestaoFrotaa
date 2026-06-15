using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GestoreDeFrotas.Services
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuditoriaService auditoriaService)
        {
            await _next(context);

            var username = (context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.Name)?.Value
                : "Anónimo") ?? "Anónimo";

            var rota = context.Request.Path.ToString();
            var metodo = context.Request.Method;
            var statusCode = context.Response.StatusCode;

            var descricao = $"Request {metodo} em {rota} devolveu {statusCode}.";

            await auditoriaService.RegistarLogAsync(username, metodo, rota, descricao, statusCode);

            if (statusCode >= 400)
            {
                await auditoriaService.CriarNotificacaoAsync(
                    $"Erro {statusCode} na API",
                    $"O utilizador {username} fez {metodo} em {rota} e obteve {statusCode}.",
                    "Error"
                );
            }
        }
    }
}
