using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetNotificacoes()
        {
            var notificacoes = await _service.ObterNotificacoesAtivasAsync();
            return Ok(notificacoes);
        }

        [HttpPut("notificacoes/{id}/ler")]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            await _service.MarcarComoLidaAsync(id);
            return NoContent();
        }
    }
}
