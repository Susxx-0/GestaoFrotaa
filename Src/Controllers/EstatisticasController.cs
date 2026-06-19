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

            decimal totalManutencoes = await _context.RegistosManutencao
                .SumAsync(m => (decimal?)m.Custo) ?? 0;

            decimal totalAbastecimentos = await _context.Abastecimentos
                .SumAsync(a => (decimal?)a.CustoTotal) ?? 0;

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
            if (v == null)
                return NotFound("Veículo não encontrado.");

            decimal custosManutencao = await _context.RegistosManutencao
                .Where(m => m.VeiculoId == id)
                .SumAsync(m => (decimal?)m.Custo) ?? 0;

            var abastecimentos = await _context.Abastecimentos
                .Where(a => a.VeiculoId == id)
                .ToListAsync();

            decimal custosAbastecimento = abastecimentos.Sum(a => (decimal)a.CustoTotal);
            decimal totalLitros = abastecimentos.Sum(a => (decimal)a.Litros);

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
