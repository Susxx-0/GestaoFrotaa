using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Services;
using GestaoDeFrotas.Models;
using System;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AbastecimentosController : ControllerBase
    {
        private readonly AbastecimentosService _service;
        private readonly IValidator<Abastecimento> _validator; // Injeção do validador

        public AbastecimentosController(AbastecimentosService service, IValidator<Abastecimento> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.ObterTodosAsync());
        }

        [HttpGet("veiculo/{id}")]
        public async Task<IActionResult> GetByVeiculo(int id)
        {
            return Ok(await _service.ObterPorVeiculoAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Abastecimento ab)
        {
            // Executa a validação do FluentValidation antes de avançar
            var validationResult = await _validator.ValidateAsync(ab);

            if (!validationResult.IsValid)
            {
                // Devolve erro 400 com os detalhes das falhas
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            try
            {
                var novo = await _service.AdicionarAsync(ab);
                return Ok(novo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}