using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GestoreDeFrotas.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var user = context.User?.Identity?.Name ?? "Anónimo";

            _logger.LogInformation("➡️ Request: {method} {path} | User: {user}",
                request.Method, request.Path, user);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro inesperado no processamento do request.");
                throw;
            }

            stopwatch.Stop();

            _logger.LogInformation("⬅️ Response: {statusCode} | Tempo: {ms}ms",
                context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
