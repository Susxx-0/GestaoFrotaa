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

        [HttpGet]
        public async Task<IActionResult> ObterTodos(string? pesquisa = null, string? ordenarPor = null)
        {
            var lista = await _vehicleService.ObterTodosAsync(pesquisa, ordenarPor);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var veiculo = await _vehicleService.ObterPorIdAsync(id);
            if (veiculo == null)
                return NotFound("Veículo não encontrado.");

            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Veiculo veiculo)
        {
            var novo = await _vehicleService.CriarAsync(veiculo);
            return CreatedAtAction(nameof(ObterPorId), new { id = novo.Id }, novo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Veiculo dados)
        {
            var atualizado = await _vehicleService.AtualizarAsync(id, dados);
            if (atualizado == null)
                return NotFound("Veículo não encontrado.");

            await _vehicleService.RegistarHistoricoAsync(id, "Atualização de dados");
            return Ok(atualizado);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> AlterarEstado(int id, [FromQuery] string estado)
        {
            var ok = await _vehicleService.AlterarEstadoAsync(id, estado);
            if (!ok)
                return NotFound("Veículo não encontrado.");

            await _vehicleService.RegistarHistoricoAsync(id, $"Estado alterado para {estado}");
            return Ok($"Estado atualizado para {estado}");
        }

        [HttpPatch("{id}/desativar")]
        public async Task<IActionResult> Desativar(int id)
        {
            var ok = await _vehicleService.DesativarAsync(id);
            if (!ok)
                return NotFound("Veículo não encontrado.");

            await _vehicleService.RegistarHistoricoAsync(id, "Veículo desativado");
            return Ok("Veículo desativado com sucesso.");
        }

        [HttpPatch("{id}/ativar")]
        public async Task<IActionResult> Ativar(int id)
        {
            var ok = await _vehicleService.AtivarAsync(id);
            if (!ok)
                return NotFound("Veículo não encontrado.");

            await _vehicleService.RegistarHistoricoAsync(id, "Veículo ativado");
            return Ok("Veículo ativado com sucesso.");
        }

        [HttpGet("{id}/validar-viagem")]
        public async Task<IActionResult> ValidarParaViagem(int id)
        {
            await _vehicleService.ValidarDisponibilidadeParaViagem(id);
            return Ok("Veículo apto para iniciar viagem.");
        }

        [HttpGet("{id}/form")]
        public async Task<IActionResult> ObterFormularioEdicao(int id)
        {
            var veiculo = await _vehicleService.ObterPorIdAsync(id);
            if (veiculo == null)
                return NotFound("Veículo não encontrado.");

            return Ok(new
            {
                dados = veiculo,
                opcoes = new
                {
                    categorias = new[] { "Empresa", "Outros" },
                    estados = new[] { "Disponível", "Indisponível", "Em Manutenção" },
                    cores = new[] { "Preto", "Branco", "Cinza", "Azul", "Vermelho" }
                }
            });
        }







    }
}
