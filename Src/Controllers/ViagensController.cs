using GestoreDeFrotas.Models.DTOs;
using GestoreDeFrotas.Services.Viagens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViagensController : ControllerBase
    {
        private readonly ViagensService _service;
        private readonly AppDbContext _context;

        public ViagensController(ViagensService service, AppDbContext context)
        {
            _service = service;
            _context = context;
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

                //  Obter utilizador
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == condutorPrincipalId);

                if (user == null)
                    return BadRequest(new { mensagem = "Utilizador não encontrado." });

                //   Verificar se tem carta
                if (user.CartaConducaoValidade == null)
                    return BadRequest(new { mensagem = "O utilizador não tem carta registada." });

                //  Verificar se está expirada
                if (user.CartaConducaoValidade < DateTime.Today)
                    return BadRequest(new { mensagem = "A carta de condução está expirada. Não é possível iniciar a viagem." });

                //  Verificar categoria do veículo
                var veiculo = await _context.Veiculos.FindAsync(veiculoId);

                if (veiculo == null)
                    return BadRequest(new { mensagem = "Veículo não encontrado." });

                if (!string.IsNullOrEmpty(veiculo.CategoriaUsuario) &&
                    veiculo.CategoriaUsuario != user.CartaConducaoCategoria)
                {
                    return BadRequest(new
                    {
                        mensagem = $"A categoria da carta ({user.CartaConducaoCategoria}) não permite conduzir este veículo ({veiculo.CategoriaUsuario})."
                    });
                }

                // Se tudo OK:iniciar viagem
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
