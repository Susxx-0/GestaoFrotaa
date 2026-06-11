using GestoreDeFrotas.Filtro;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ViagensService _viagensService;

        public ViagensController(ViagensService viagensService)
        {
            _viagensService = viagensService;
        }

        // 1. INICIAR VIAGEM
        [HttpPost("iniciar/{veiculoId}")]
        [TypeFilter(typeof(AuditarAcaoFilter))]
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

        // 2. FINALIZAR VIAGEM (Corrigido de acordo com a assinatura real do vosso serviço)
        [HttpPost("finalizar/{viagemId}")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Outros")]
        [TypeFilter(typeof(AuditarAcaoFilter))]
        public async Task<IActionResult> FinalizarViagem(int viagemId, [FromBody] EntregaViagemDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { mensagem = "Os dados de encerramento são obrigatórios." });
                }

                // Chama o serviço passando apenas os 2 parâmetros que ele aceita: viagemId e observacoesEntrega
                await _viagensService.FinalizarViagemAsync(viagemId, dto.ObservacoesEntrega);

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
                return Ok(new { mensagem = $"Prorrogação de {horas} horas registada." });
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

    public class EntregaViagemDto
    {
        public int KmFinais { get; set; }
        public string? ObservacoesEntrega { get; set; }
    }
}