using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services.Veiculos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AbastecimentosController : ControllerBase
    {
        private readonly AbastecimentosService _service;
        private readonly IValidator<Abastecimento> _validator;

        public AbastecimentosController(AbastecimentosService service, IValidator<Abastecimento> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            return Ok(await _service.ObterTodosAsync());
        }

        [HttpGet("veiculo/{id}")]
        public async Task<IActionResult> ObterPorVeiculo(int id)
        {
            return Ok(await _service.ObterPorVeiculoAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Abastecimento ab)
        {
            var valid = await _validator.ValidateAsync(ab);
            if (!valid.IsValid)
                return BadRequest(valid.Errors.Select(e => e.ErrorMessage));

            try
            {
                var novo = await _service.CriarAsync(ab);
                return Ok(novo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}
