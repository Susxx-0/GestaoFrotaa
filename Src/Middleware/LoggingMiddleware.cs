using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Http;
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

            var user = context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Anónimo"
                : "Anónimo";

            await auditoriaService.RegistarLogAsync(
                user,
                context.Request.Method,
                context.Request.Path.ToString(),
                $"Pedido HTTP {context.Request.Method} processado em {context.Request.Path}",
                context.Response.StatusCode
            );
        }
    }
}