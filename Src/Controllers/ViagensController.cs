using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestoreDeFrotas.Services; // Certifica-se de que aponta para os Serviços
using GestoreDeFrotas.Models;   // Caso precise de aceder à entidade Viagem
using GestoreDeFrotas.Filtro;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViagensController : ControllerBase
    {
    
    
        private readonly ViagensService
           _viagensService;

        public ViagensController(ViagensService viagensService)
        {
            _viagensService = viagensService;
        }

        // 1. INICIAR VIAGEM
        [HttpPost("iniciar/{veiculoId}")]
        [TypeFilter(typeof(AuditarAcaoFilter))] // TypeFilter resolve a injeção de dependência do AuditoriaService!
        public async Task<IActionResult> Iniciar(int veiculoId)
        {
            var condutorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            try
            {
                var viagem = await _viagensService.IniciarViagemAsync(veiculoId, condutorId);
                return Ok(viagem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // 2. FINALIZAR VIAGEM
        [HttpPost("finalizar/{viagemId}")]
        [TypeFilter(typeof(AuditarAcaoFilter))]
        public async Task<IActionResult> Finalizar(int viagemId, [FromBody] string observacoes)
        {
            try
            {
                await _viagensService.FinalizarViagemAsync(viagemId, observacoes);
                return Ok(new { mensagem = "Viagem finalizada com sucesso e veículo libertado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // 3. PEDIR MAIS TEMPO
        [HttpPost("{id}/prorrogacao")]
        [TypeFilter(typeof(AuditarAcaoFilter))]
        public async Task<IActionResult> SolicitarProrrogacao(int id, [FromQuery] int horas = 2)
        {
            try
            {
                await _viagensService.SolicitarMaisTempoAsync(id, horas);
                return Ok(new { mensagem = $"Prorrogação de {horas} horas registada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // 4. VERIFICAÇÃO AUTOMÁTICA
        [HttpPost("verificar-atrasos")]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarAtrasos()
        {
            try
            {
                await _viagensService.VerificarEAlertarAtrasosAsync();
                return Ok(new { mensagem = "Verificação concluída." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}