using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Services;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumo")]
        public async Task<IActionResult> GetResumo()
        {
            var resumo = await _dashboardService.ObterResumoGeralAsync();
            return Ok(resumo);
        }

        [HttpGet("combustivel")]
        public async Task<IActionResult> GetCombustivel(
            [FromServices] DashboardCombustivelService combustivelService)
        {
            var dados = await combustivelService.ObterEstatisticasAsync();
            return Ok(dados);
        }

        [HttpGet("utilizacao")]
        public async Task<IActionResult> GetUtilizacao(
            [FromServices] DashboardUtilizacaoService utilizacaoService)
        {
            var dados = await utilizacaoService.ObterEstatisticasAsync();
            return Ok(dados);
        }

        [HttpGet("alertas")]
        public async Task<IActionResult> GetAlertas(
            [FromServices] DashboardAlertasService alertasService)
        {
            var dados = await alertasService.ObterAlertasAsync();
            return Ok(dados);
        }

        // 🔥 NOVO ENDPOINT — MANUTENÇÃO INTELIGENTE
        [HttpGet("manutencao")]
        public async Task<IActionResult> GetManutencao(
            [FromServices] DashboardManutencaoService manutencaoService)
        {
            var dados = await manutencaoService.ObterEstadoManutencoesAsync();
            return Ok(dados);
        }
    }
}
