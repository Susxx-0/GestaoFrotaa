using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly VeiculosService _veiculoService;

        public VeiculosController(VeiculosService veiculoService)
        {
            _veiculoService = veiculoService;
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> ObterAtivos([FromQuery] string? ordenarPor)
        {
            return Ok(await _veiculoService.ObterTodosAtivosAsync(ordenarPor));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var v = await _veiculoService.ObterPorIdAsync(id);
            if (v == null) return NotFound(new { mensagem = "Veículo não encontrado." });
            return Ok(v);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Criar([FromBody] Veiculo v)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _veiculoService.CriarAsync(v);
            return CreatedAtAction(nameof(ObterPorId), new { id = v.Id }, v);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Veiculo v)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sucesso = await _veiculoService.AtualizarAsync(id, v);
            if (!sucesso) return NotFound(new { mensagem = "Veículo não encontrado." });
            return NoContent();
        }

        // 🔥 Endpoint: Veículos com manutenção atrasada
        [HttpGet("manutencao-atrasada")]
        public async Task<IActionResult> ObterManutencaoAtrasada()
        {
            return Ok(await _veiculoService.ObterManutencaoAtrasadaAsync());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _veiculoService.EliminarAsync(id);
            return NoContent();
        }
    }
}
