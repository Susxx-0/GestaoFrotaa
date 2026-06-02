using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Models;
using GestaoDeFrotas.Services;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly VeiculosService _veiculosService;

        public VeiculosController(VeiculosService veiculosService)
        {
            _veiculosService = veiculosService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? marca,
            [FromQuery] string? modelo,
            [FromQuery] string? matricula,
            [FromQuery] string? estado,
            [FromQuery] string? cor,
            [FromQuery] int? anoMin,
            [FromQuery] int? anoMax)
        {
            var veiculos = await _veiculosService.FiltrarAsync(
                marca, modelo, matricula, estado, cor, anoMin, anoMax
            );

            return Ok(veiculos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create([FromBody] Veiculo veiculo)
        {
            await _veiculosService.AdicionarAsync(veiculo);
            return Ok(veiculo);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _veiculosService.EliminarAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
