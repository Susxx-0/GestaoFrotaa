using GestoreDeFrotas.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstatisticasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstatisticasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumo-geral")]
        public async Task<IActionResult> ResumoGeral()
        {
            var totalVeiculos = await _context.Veiculos.CountAsync();
            var totalManutencoes = await _context.RegistosManutencao.SumAsync(m => (decimal)m.Custo);
            var totalAbastecimentos = await _context.Abastecimentos.SumAsync(a => (decimal)a.CustoTotal);
            return Ok(new
            {
                totalVeiculos,
                custoTotalFrota = totalManutencoes + totalAbastecimentos,
                detalheCustos = new
                {
                    manutencoes = totalManutencoes,
                    abastecimentos = totalAbastecimentos
                }
            });
        }

        [HttpGet("veiculo/{id}")]
        public async Task<IActionResult> EstatisticasVeiculo(int id)
        {
            var v = await _context.Veiculos.FindAsync(id);
            if (v == null) return NotFound();

            var custosManutencao = await _context.RegistosManutencao
                .Where(m => m.VeiculoId == id)
                .SumAsync(m => (double)m.Custo);

            var abastecimentos = await _context.Abastecimentos
                .Where(a => a.VeiculoId == id)
                .ToListAsync();

            var custosAbastecimento = abastecimentos.Sum(a => a.CustoTotal);
            var totalLitros = abastecimentos.Sum(a => a.Litros);

            return Ok(new
            {
                v.Id,
                v.Matricula,
                v.Modelo,
                v.Marca,
                v.Ano,
                v.Estado,
                custoTotalAcumulado = custosManutencao + custosAbastecimento,
                custosSeparados = new
                {
                    manutencao = custosManutencao,
                    abastecimento = custosAbastecimento
                },
                totalAbastecimentosRealizados = abastecimentos.Count,
                totalLitrosConsumidos = totalLitros
            });
        }
    }
}
