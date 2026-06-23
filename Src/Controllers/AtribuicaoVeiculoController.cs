using GestoreDeFrotas.Models.Dtos;
using GestoreDeFrotas.Services.Veiculos;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/atribuicoes")]
    public class AtribuicaoVeiculoController : ControllerBase
    {
        private readonly AtribuicaoVeiculoService _service;

        public AtribuicaoVeiculoController(AtribuicaoVeiculoService service)
        {
            _service = service;
        }

        [HttpPost("atribuir")]
        public async Task<IActionResult> Atribuir([FromBody] AtribuirVeiculoDTO dto)
        {
            return Ok(await _service.AtribuirAsync(dto.UserId, dto.VeiculoId));
        }

        [HttpPost("remover")]
        public async Task<IActionResult> Remover([FromBody] RemoverAtribuicaoDTO dto)
        {
            return Ok(await _service.RemoverAtribuicaoAsync(dto.VeiculoId));
        }

        [HttpGet("veiculo/{veiculoId}")]
        public async Task<IActionResult> ObterAtribuicaoAtual(int veiculoId)
        {
            return Ok(await _service.ObterAtribuicaoAtualAsync(veiculoId));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> ObterVeiculosDoUser(int userId)
        {
            return Ok(await _service.ObterVeiculosDoUserAsync(userId));
        }
    }
}
