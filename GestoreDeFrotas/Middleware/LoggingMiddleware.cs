using GestaoDeFrotas.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Middleware
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

            if (context.Request.Method != "GET")
            {
                var username = context.User.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst(ClaimTypes.Name)?.Value
                    : "Anónimo";

                var rota = context.Request.Path;
                var metodo = context.Request.Method;
                var statusCode = context.Response.StatusCode;

                string descricao = $"Executou uma operação no endpoint {rota}";

                if (statusCode >= 400)
                {
                    await auditoriaService.CriarNotificacaoAsync(
                        titulo: $"Erro detetado ({statusCode})",
                        mensagem: $"O utilizador {username} falhou ao tentar fazer {metodo} em {rota}.",
                        tipo: "Error"
                    );
                }

                await auditoriaService.RegistarLogAsync(username, metodo, rota, descricao, statusCode);
            }
        }
    }
}