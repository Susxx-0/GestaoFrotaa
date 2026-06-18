using GestoreDeFrotas.Models;
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

        // ============================
        // 1. OBTER TODOS com pesquisa
        // ============================
        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string? pesquisa = null,
            [FromQuery] string? ordenarPor = null)
        {
            var lista = await _vehicleService.ObterTodosAsync(pesquisa, ordenarPor);
            return Ok(lista);
        }

        // ============================
        // 2. OBTER POR ID
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var veiculo = await _vehicleService.ObterPorIdAsync(id);
            if (veiculo == null)
                return NotFound("Veículo não encontrado.");

            return Ok(veiculo);
        }

        // ============================
        // 3. CRIAR VEÍCULO
        // ============================
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Veiculo veiculo)
        {
            var novo = await _vehicleService.CriarAsync(veiculo);
            return CreatedAtAction(nameof(ObterPorId), new { id = novo.Id }, novo);
        }

        // ============================
        // 4. ATUALIZAR VEÍCULO
        // ============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Veiculo dados)
        {
            var ok = await _vehicleService.AtualizarAsync(id, dados);
            if (!ok)
                return NotFound("Veículo não encontrado.");

            await _vehicleService.RegistarHistoricoAsync(id, "Atualização de dados");
            return Ok("Veículo atualizado com sucesso.");
        }

        // ============================
        // 5. ALTERAR ESTADO DO VEÍCULO
        // ============================
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> AlterarEstado(int id, [FromQuery] string estado)
        {
            try
            {
                var ok = await _vehicleService.AlterarEstadoAsync(id, estado);
                if (!ok)
                    return NotFound("Veículo não encontrado.");

                await _vehicleService.RegistarHistoricoAsync(id, $"Estado alterado para {estado}");
                return Ok($"Estado atualizado para {estado}");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================
        // 6. DESATIVAR VEÍCULO
        // ============================
        [HttpPatch("{id}/desativar")]
        public async Task<IActionResult> Desativar(int id)
        {
            var ok = await _vehicleService.DesativarAsync(id);
            if (!ok)
                return NotFound("Veículo não encontrado.");

            await _vehicleService.RegistarHistoricoAsync(id, "Veículo desativado");
            return Ok("Veículo desativado com sucesso.");
        }

        // ============================
        // 7. VALIDAR DISPONIBILIDADE PARA VIAGEM
        // ============================
        [HttpGet("{id}/validar-viagem")]
        public async Task<IActionResult> ValidarParaViagem(int id)
        {
            try
            {
                await _vehicleService.ValidarDisponibilidadeParaViagem(id);
                return Ok("Veículo apto para iniciar viagem.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
