using Microsoft.AspNetCore.Mvc;
using GestoreDeFrotas.Services; 
using GestoreDeFrotas.Filtro;   
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers 

{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // MANTIDO: O teu endpoint original de resumo geral
        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumoGeral()
        {
            var resultado = await _dashboardService.ObterResumoGeralAsync();
            return Ok(resultado);
        }

        // NOVO: Endpoint para listar os veículos na rua com a data de início real
        [HttpGet("veiculos-em-uso")]
        public async Task<IActionResult> ObterVeiculosEmUso()
        {
            var resultado = await _dashboardService.ObterVeiculosEmUsoDashboardAsync();
            return Ok(resultado);
        }

        // NOVO: Endpoint para listar alertas de inspeção (IPO) críticas
        [HttpGet("alertas-ipo")]
        public async Task<IActionResult> ObterAlertasIpo()
        {
            var resultado = await _dashboardService.ObterAlertasIpoDashboardAsync();
            return Ok(resultado);
        }
    }
}