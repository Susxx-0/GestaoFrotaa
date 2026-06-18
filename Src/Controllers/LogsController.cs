using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        private readonly AuditoriaService _auditoria;

        public LogsController(AuditoriaService auditoria)
        {
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            return Ok(await _auditoria.ObterTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var log = await _auditoria.ObterPorIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpPost("teste")]
        public async Task<IActionResult> CriarTeste()
        {
            await _auditoria.RegistarLogAsync(
                user: "Teste",
                metodo: "LogsController.CriarTeste",
                rota: "/api/logs/teste",
                descricao: "Log de teste criado.",
                metodoHttp: "POST",
                status: 200
            );

            return Ok("Log criado.");
        }
    }
}
