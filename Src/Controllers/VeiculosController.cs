using GestoreDeFrotas.Dtos.Veiculos;
using GestoreDeFrotas.Services.Veiculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/veiculos")]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _vehicleService;

        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(string? pesquisa = null, string? ordenarPor = null)
        {
            return Ok(await _vehicleService.ObterTodosAsync(pesquisa, ordenarPor));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var v = await _vehicleService.ObterPorIdAsync(id);
            return v == null ? NotFound("Veículo não encontrado.") : Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] VeiculoCreateDTO dto)
        {
            var novo = await _vehicleService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = novo.Id }, novo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] VeiculoUpdateDTO dto)
        {
            var atualizado = await _vehicleService.AtualizarAsync(id, dto);
            return atualizado == null ? NotFound("Veículo não encontrado.") : Ok(atualizado);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> AlterarEstado(int id, [FromQuery] string estado)
        {
            var ok = await _vehicleService.AlterarEstadoAsync(id, estado);
            return ok ? Ok("Estado atualizado.") : NotFound("Veículo não encontrado.");
        }

        [HttpPatch("{id}/desativar")]
        public async Task<IActionResult> Desativar(int id)
        {
            var ok = await _vehicleService.DesativarAsync(id);
            return ok ? Ok("Veículo desativado.") : NotFound("Veículo não encontrado.");
        }

        [HttpPatch("{id}/ativar")]
        public async Task<IActionResult> Ativar(int id)
        {
            var ok = await _vehicleService.AtivarAsync(id);
            return ok ? Ok("Veículo ativado.") : NotFound("Veículo não encontrado.");
        }
    }
}
