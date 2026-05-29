using Microsoft.Extensions.Logging;

namespace GestaoDeFrotas.Services
{
    public class LoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void Info(string mensagem)
        {
            _logger.LogInformation(mensagem);
        }

        public void Erro(string mensagem, Exception ex)
        {
            _logger.LogError(ex, mensagem);
        }
    }
}
