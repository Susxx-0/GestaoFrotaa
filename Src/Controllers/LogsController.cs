using GestaoDeFrotas.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly AuditoriaService _auditoriaService;

        public LogsController(AuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _auditoriaService.ObterLogsAsync();
            return Ok(logs);
        }
    }
}
