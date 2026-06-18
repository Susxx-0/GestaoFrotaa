using GestoreDeFrotas.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/dashboard/resumo
        [HttpGet("resumo")]
        public async Task<IActionResult> ObterResumo()
        {
            var resumo = await _dashboardService.ObterResumoGeralAsync();
            return Ok(resumo);
        }

        // GET: api/dashboard/veiculos-uso
        [HttpGet("veiculos-uso")]
        public async Task<IActionResult> ObterVeiculosEmUso()
        {
            var dados = await _dashboardService.ObterVeiculosEmUsoDashboardAsync();
            return Ok(dados);
        }

        // GET: api/dashboard/alertas-ipo
        [HttpGet("alertas-ipo")]
        public async Task<IActionResult> ObterAlertasIpo()
        {
            var alertas = await _dashboardService.ObterAlertasIpoDashboardAsync();
            return Ok(alertas);
        }
    }
}
