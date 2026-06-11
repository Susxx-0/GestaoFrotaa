using GestoreDeFrotas.Data;
using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("logs")]
        public async Task<IActionResult> ObterLogs([FromServices] AppDbContext context)
        {
            var logs = await context.LogsSistema
                .OrderByDescending(l => l.Id) 
                .Take(100)
                .ToListAsync();

            return Ok(logs);
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