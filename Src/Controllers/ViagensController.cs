using GestoreDeFrotas.Models.DTOs;
using GestoreDeFrotas.Services.Viagens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GestoreDeFrotas.Models;
namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViagensController : ControllerBase
    {
        private readonly ViagensService _service;

        public ViagensController(ViagensService service)
        {
            _service = service;
        }

        // INICIAR VIAGEM
        [HttpPost("iniciar/{veiculoId}")]
        public async Task<IActionResult> Iniciar(int veiculoId, [FromQuery] string? condutorSecundarioId = null)
        {
            try
            {
                var condutorPrincipalId =
                    User.Identity?.Name ??
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    "Condutor Anónimo";

                var viagem = await _service.IniciarViagemAsync(
                    veiculoId,
                    condutorPrincipalId,
                    condutorSecundarioId
                );

                return Ok(viagem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // FINALIZAR VIAGEM
        [HttpPost("finalizar/{viagemId}")]
        public async Task<IActionResult> Finalizar(int viagemId, [FromBody] FinalizarViagemDto dto)
        {
            try
            {
                await _service.FinalizarViagemAsync(viagemId, dto.KmFinais, dto.ObservacoesEntrega);
                return Ok(new { mensagem = "Viagem finalizada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // PRORROGAÇÃO
        [HttpPost("{id}/prorrogacao")]
        public async Task<IActionResult> Prorrogar(int id, [FromQuery] int horas = 2)
        {
            var ok = await _service.SolicitarMaisTempoAsync(id, horas);
            if (!ok)
                return BadRequest(new { mensagem = "Não foi possível prorrogar a viagem." });

            return Ok(new { mensagem = $"Prorrogação de {horas} horas registada." });
        }

        // VERIFICAR ATRASOS
        [HttpPost("verificar-atrasos")]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarAtrasos()
        {
            await _service.VerificarEAlertarAtrasosAsync();
            return Ok(new { mensagem = "Verificação concluída." });
        }

        // HISTÓRICO
        [HttpGet("historico/{veiculoId}")]
        public async Task<IActionResult> Historico(int veiculoId)
        {
            var historico = await _service.ObterHistoricoViagensAsync(veiculoId);
            return Ok(historico);
        }
    }
}
