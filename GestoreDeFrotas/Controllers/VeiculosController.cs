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

        // O construtor agora recebe o Serviço e não o DbContext
        public VeiculosController(VeiculosService veiculosService)
        {
            _veiculosService = veiculosService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> GetAll()
        {
            var veiculos = await _veiculosService.ObterTodosAsync();
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