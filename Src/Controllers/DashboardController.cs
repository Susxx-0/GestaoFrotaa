using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Data;
using Microsoft.EntityFrameworkCore; // Necessário para o CountAsync
using System;
using System.Linq;
using System.Threading.Tasks; // Necessário para Task<IActionResult>

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEstatisticas()
        {
            var hoje = DateTime.Now;

            // Usamos await e CountAsync para não bloquear o servidor
            var estatisticas = new
            {
                TotalVeiculos = await _context.Veiculos.CountAsync(),
                Disponiveis = await _context.Veiculos.CountAsync(v => v.Estado == "Disponível"),
                Alugados = await _context.Veiculos.CountAsync(v => v.Estado == "Alugado"),
                EmManutencao = await _context.Veiculos.CountAsync(v => v.Estado == "Em Manutenção"),

                TotalGastoManutencoes = await _context.RegistosManutencao.SumAsync(m => (decimal?)m.Custo) ?? 0,
                NumeroTotalManutencoes = await _context.RegistosManutencao.CountAsync(),

                TotalDocumentos = await _context.DocumentosVeiculos.CountAsync(),
                DocumentosExpirados = await _context.DocumentosVeiculos.CountAsync(d => d.DataValidade != null && d.DataValidade < hoje)
            };

            return Ok(estatisticas);
        }
    }
}