using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services.Abastecimentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/abastecimentos")]
    [Authorize]
    public class AbastecimentoController : ControllerBase
    {
        private readonly AbastecimentoService _service;

        public AbastecimentoController(AbastecimentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            return Ok(await _service.ObterTodosAsync());
        }

        [HttpGet("veiculo/{veiculoId}")]
        public async Task<IActionResult> ObterPorVeiculo(int veiculoId)
        {
            return Ok(await _service.ObterPorVeiculoAsync(veiculoId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var item = await _service.ObterPorIdAsync(id);
            if (item == null)
                return NotFound("Abastecimento não encontrado.");

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Abastecimento abastecimento)
        {
            var novo = await _service.CriarAsync(abastecimento);
            return CreatedAtAction(nameof(ObterPorId), new { id = novo.Id }, novo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Abastecimento dados)
        {
            var atualizado = await _service.AtualizarAsync(id, dados);
            if (atualizado == null)
                return NotFound("Abastecimento não encontrado.");

            return Ok(atualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Apagar(int id)
        {
            var ok = await _service.ApagarAsync(id);
            if (!ok)
                return NotFound("Abastecimento não encontrado.");

            return Ok("Abastecimento removido com sucesso.");
        }


        [HttpGet("{id}/form")]
        public async Task<IActionResult> ObterFormularioEdicao(int id)
        {
            var abastecimento = await _service.ObterPorIdAsync(id);
            if (abastecimento == null)
                return NotFound("Abastecimento não encontrado.");

            return Ok(new
            {
                dados = abastecimento,
                opcoes = new
                {
                    combustiveis = new[] { "Gasolina 95", "Gasolina 98", "Gasóleo", "GPL", "Elétrico" },
                    postos = new[] { "Galp", "BP", "Repsol", "CEPSA", "Prio" }
                }
            });
        }








    }
}
