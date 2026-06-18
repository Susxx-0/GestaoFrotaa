using GestoreDeFrotas.Services.Sistema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
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

        [HttpGet("notificacoes")]
        public async Task<IActionResult> ObterNotificacoes()
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
