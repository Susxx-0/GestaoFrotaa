using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstatisticasController : ControllerBase
    {
 
        private readonly GestaoDeFrotas.Data.AppDbContext _context;

        public EstatisticasController(GestaoDeFrotas.Data.AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/estatisticas/resumo-geral
        [HttpGet("resumo-geral")]
        public async Task<IActionResult> GetResumoGeral()
        {
            var totalVehiculos = await _context.Veiculos.CountAsync();

            
            var totalGastoManutencoes = await _context.RegistosManutencao.SumAsync(m => (double)m.Custo);
            var totalGastoAbastecimentos = await _context.Abastecimentos.SumAsync(a => a.CustoTotal);

            return Ok(new
            {
                totalVehiculos,
                custoTotalFrota = totalGastoManutencoes + totalGastoAbastecimentos,
                detalheCustos = new
                {
                    manutencoes = totalGastoManutencoes,
                    abastecimentos = totalGastoAbastecimentos
                }
            });
        }

        // 2. GET: api/estatisticas/veiculo/{veiculoId}
        [HttpGet("veiculo/{veiculoId}")]
        public async Task<IActionResult> GetEstatisticasVeiculo(int veiculoId)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null) return NotFound(new { mensagem = "Veículo não encontrado." });

            var custosManutencao = await _context.RegistosManutencao
                .Where(m => m.VeiculoId == veiculoId)
                .SumAsync(m => (double)m.Custo);

            var abastecimentos = await _context.Abastecimentos
                .Where(a => a.VeiculoId == veiculoId)
                .ToListAsync();

            var custosAbastecimento = abastecimentos.Sum(a => a.CustoTotal);
            var totalLitros = abastecimentos.Sum(a => a.Litros);

            return Ok(new
            {
                veiculoId,
                veiculo.Matricula,
                veiculo.Modelo,
                veiculo.Marca,
                veiculo.Ano,
                veiculo.Estado,
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