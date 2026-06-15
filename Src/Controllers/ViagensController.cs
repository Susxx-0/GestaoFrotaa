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
        public async Task<IActionResult> IniciarViagem(int veiculoId, [FromQuery] string? condutorSecundarioId = null)
        {
            try
            {
                // Tenta apanhar o NameIdentifier (geralmente o ID ou Username nas claims do JWT)
                var condutorPrincipalId = User.Identity?.Name
                                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                          ?? "Condutor Anónimo";

                var novaViagem = await _viagensService.IniciarViagemAsync(veiculoId, condutorPrincipalId, condutorSecundarioId);
                return Ok(novaViagem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // 2. FINALIZAR VIAGEM
        [HttpPost("finalizar/{viagemId}")]
        public async Task<IActionResult> FinalizarViagem(int viagemId, [FromBody] FinalizarViagemDto dados)
        {
            try
            {
                // Passa o ID da viagem, os KM finais e as observações para o serviço
                await _viagensService.FinalizarViagemAsync(viagemId, dados.KmFinais, dados.ObservacoesEntrega);
                return Ok(new { mensagem = "Viagem finalizada e veículo libertado com sucesso!" });
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
                return Ok(new { message = $"Prorrogação de {horas} horas registada." });
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




        // 5. OBTER HISTÓRICO DE VIAGENS DE UM VEÍCULO
        [HttpGet("historico/{veiculoId}")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> ObterHistorico(int veiculoId)
        {
            try
            {
                var historico = await _viagensService.ObterHistoricoViagensAsync(veiculoId);
                return Ok(historico);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao carregar o histórico de viagens.", detalhe = ex.Message });
            }
        }

        // 6. PRÉ-VISUALIZAR RELATÓRIO NO SWAGGER / BROWSER
        [HttpGet("relatorio/visualizar/{veiculoId}")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> VisualizarRelatorio(int veiculoId, [FromQuery] string formato = "pdf")
        {
            try
            {
                byte[] ficheiroBytes;
                string contentType;
                string extensao;

                if (formato.ToLower() == "excel")
                {
                    // TODO: Substitui pela chamada ao teu serviço real que gera o Excel do veículo
                    // Exemplo: ficheiroBytes = await _veiculoService.GerarExcelHistoricoAsync(veiculoId);
                    ficheiroBytes = new byte[0];
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    extensao = "xlsx";
                }
                else // PDF
                {
                    // TODO: Substitui pela chamada ao teu serviço real que já gera o teu PDF atual
                    // Exemplo: ficheiroBytes = await _veiculoService.GerarPdfHistoricoAsync(veiculoId);
                    ficheiroBytes = new byte[0];
                    contentType = "application/pdf";
                    extensao = "pdf";
                }

                // Se o serviço ainda não devolver bytes, evita enviar um ficheiro corrompido
                if (ficheiroBytes == null || ficheiroBytes.Length == 0)
                {
                    return BadRequest(new { mensagem = "O serviço de relatórios ainda não gerou dados para este veículo." });
                }

                // O cabeçalho "inline" diz ao Swagger/Browser para MOSTRAR o ficheiro em vez de o baixar
                var contentDisposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("inline")
                {
                    FileName = $"Relatorio_Veiculo_{veiculoId}.{extensao}"
                };
                Response.Headers.Add(Microsoft.Net.Http.Headers.HeaderNames.ContentDisposition, contentDisposition.ToString());

                return File(ficheiroBytes, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao gerar a pré-visualização.", detalhe = ex.Message });
            }
        }


        public class FinalizarViagemDto
        {
            public int KmFinais { get; set; }
            public string? ObservacoesEntrega { get; set; }
        }
    }
}
