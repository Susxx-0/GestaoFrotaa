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

        // ============================
        // RESUMO GERAL DA FROTA
        // ============================
        [HttpGet("resumo-geral")]
        public async Task<IActionResult> ResumoGeral()
        {
            var totalVeiculos = await _context.Veiculos.CountAsync();

            decimal totalManutencoes = await _context.RegistosManutencao
                .SumAsync(m => m.Custo);

            decimal totalAbastecimentos = await _context.Abastecimentos
                .SumAsync(a => a.CustoTotal);

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

        // ============================
        // ESTATÍSTICAS POR VEÍCULO
        // ============================
        [HttpGet("veiculo/{id}")]
        public async Task<IActionResult> EstatisticasVeiculo(int id)
        {
            var v = await _context.Veiculos.FindAsync(id);
            if (v == null)
                return NotFound("Veículo não encontrado.");

            // CUSTOS DE MANUTENÇÃO (decimal)
            decimal custosManutencao = await _context.RegistosManutencao
                .Where(m => m.VeiculoId == id)
                .SumAsync(m => m.Custo);

            // LISTA DE ABASTECIMENTOS
            var abastecimentos = await _context.Abastecimentos
                .Where(a => a.VeiculoId == id)
                .ToListAsync();

            // CUSTOS DE ABASTECIMENTO (decimal)
            decimal custosAbastecimento = abastecimentos.Sum(a => a.CustoTotal);

            // TOTAL DE LITROS
            decimal totalLitros = abastecimentos.Sum(a => a.Litros);

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
