using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Data;
using System.Linq;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Qualquer utilizador autenticado (com token) pode ver o painel
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEstatisticas()
        {
            var totalVeiculos = _context.Veiculos.Count();

            var estatisticas = new
            {
                TotalVeiculos = totalVeiculos,
                Disponiveis = _context.Veiculos.Count(v => v.Estado == "Disponível"),
                Alugados = _context.Veiculos.Count(v => v.Estado == "Alugado"),
                EmManutencao = _context.Veiculos.Count(v => v.Estado == "Em Manutenção"),

                TotalGastoManutencoes = _context.RegistosManutencao.Sum(m => m.Custo),
                NumeroTotalManutencoes = _context.RegistosManutencao.Count()
            };

            return Ok(estatisticas);
        }
    }
}