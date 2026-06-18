using GestoreDeFrotas.Services.Sistema;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GestoreDeFrotas.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuditoriaService auditoria)
        {
            await _next(context);

            var utilizador = context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Anónimo"
                : "Anónimo";

            await auditoria.RegistarLogAsync(
                utilizador,
                context.Request.Method,
                context.Request.Path,
                $"Pedido HTTP {context.Request.Method} processado em {context.Request.Path}",
                context.Response.StatusCode
            );
        }
    }
}
