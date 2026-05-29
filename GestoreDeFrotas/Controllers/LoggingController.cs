using Microsoft.AspNetCore.Mvc;
using GestaoDeFrotas.Services;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LoggingController : ControllerBase
    {
        private readonly LoggingService _logging;

        public LoggingController(LoggingService logging)
        {
            _logging = logging;
        }

        [HttpGet("teste")]
        public IActionResult TestarLogs()
        {
            _logging.Info("Teste de log executado com sucesso.");
            return Ok(new { mensagem = "Log registado." });
        }
    }
}
