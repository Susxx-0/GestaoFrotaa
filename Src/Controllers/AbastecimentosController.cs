using GestoreDeFrotas.Models.Dtos.Abastecimentos;
using GestoreDeFrotas.Services.Abastecimentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/abastecimentos")]
    [Authorize]
    public class AbastecimentosController : ControllerBase
    {
        private readonly AbastecimentoService _service;

        public AbastecimentosController(AbastecimentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            return Ok(await _service.ObterTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var a = await _service.ObterPorIdAsync(id);
            return a == null ? NotFound("Abastecimento não encontrado.") : Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] AbastecimentoCreateDTO dto)
        {
            var novo = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = novo.Id }, novo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AbastecimentoUpdateDTO dto)
        {
            var atualizado = await _service.AtualizarAsync(id, dto);
            return atualizado == null ? NotFound("Abastecimento não encontrado.") : Ok(atualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Apagar(int id)
        {
            var ok = await _service.ApagarAsync(id);
            return ok ? Ok("Abastecimento apagado.") : NotFound("Abastecimento não encontrado.");
        }
    }
}
