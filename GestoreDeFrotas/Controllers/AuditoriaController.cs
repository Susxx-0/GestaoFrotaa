using GestaoDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/auditoria")]
    [Authorize]
    public class AuditoriaController : ControllerBase
    {
        private readonly AuditoriaService _service;

        public AuditoriaController(AuditoriaService service)
        {
            _service = service;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(await _service.ObterLogsAsync());
        }

        [HttpGet("notificacoes")]
        public async Task<IActionResult> GetNotificacoes()
        {
            return Ok(await _service.ObterNotificacoesAtivasAsync());
        }

        [HttpPut("notificacoes/{id}/ler")]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            await _service.MarcarComoLidaAsync(id);
            return NoContent();
        }
    }
}