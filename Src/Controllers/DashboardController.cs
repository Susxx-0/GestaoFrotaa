using GestoreDeFrotas.Data;
using GestoreDeFrotas.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        private readonly AppDbContext _context;

        public DashboardController(DashboardService dashboardService, AppDbContext context)
        {
            _dashboardService = dashboardService;
            _context = context;
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

        // GET: api/dashboard/alertas-carta
        [HttpGet("alertas-carta")]
        public async Task<IActionResult> AlertasCarta()
        {
            var hoje = DateTime.Today;
            var limite = hoje.AddDays(30);

            var alertas = await _context.Users
                .Where(u => u.CartaConducaoValidade != null &&
                            u.CartaConducaoValidade <= limite)
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.CartaConducaoValidade,
                    DiasRestantes = (u.CartaConducaoValidade.Value - hoje).Days
                })
                .OrderBy(a => a.DiasRestantes)
                .ToListAsync();

            return Ok(alertas);
        }
        [HttpGet("kpis")]
        public async Task<IActionResult> ObterKpis(
         [FromServices] DashboardKpiService kpiService)
        {
            var dados = await kpiService.ObterKpisAsync();
            return Ok(dados);
        }
        [HttpGet("grafico-consumo")]
        public async Task<IActionResult> GraficoConsumo(
    [FromServices] DashboardGraficosService service)
        {
            var dados = await service.ObterConsumoMensalAsync();
            return Ok(dados);
        }

        [HttpGet("grafico-viagens")]
        public async Task<IActionResult> GraficoViagens(
            [FromServices] DashboardGraficosService service)
        {
            var dados = await service.ObterViagensMensaisAsync();
            return Ok(dados);
        }






    }
}
