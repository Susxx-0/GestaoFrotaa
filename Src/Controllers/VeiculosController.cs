using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Services;
using GestaoDeFrotas.Models;

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
        public async Task<IActionResult> GetAll(
            [FromQuery] string? marca,
            [FromQuery] string? modelo,
            [FromQuery] string? matricula,
            [FromQuery] string? estado,
            [FromQuery] string? cor,
            [FromQuery] int? anoMin,
            [FromQuery] int? anoMax)
        {
            // Passa todos os parâmetros para o serviço
            var veiculos = await _veiculosService.FiltrarAsync(marca, modelo, matricula, estado, cor, anoMin, anoMax);
            return Ok(veiculos);

        }
    }
}