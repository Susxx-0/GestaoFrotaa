using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class DashboardCombustivelService
    {
        private readonly AppDbContext _context;
        public DashboardCombustivelService(AppDbContext context) => _context = context;

        public async Task<object> ObterEstatisticasAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var totalLitros = await _context.Abastecimentos.SumAsync(a => (double?)a.Litros) ?? 0;
            var totalGasto = await _context.Abastecimentos.SumAsync(a => (double?)a.CustoTotal) ?? 0;
            var consumoMedioFrota = await _context.Abastecimentos.Where(a => a.ConsumoMedio != null).AverageAsync(a => (double?)a.ConsumoMedio) ?? 0;

    
            var historicoConsumoGrafico = Enumerable.Range(0, 6)
                .Select(i => hoje.AddMonths(-i))
                .Select(data => new
                {
                    AnoMes = data.ToString("yyyy-MM"),
                    TotalLitros = _context.Abastecimentos.Where(a => a.Data.Year == data.Year && a.Data.Month == data.Month).Sum(a => (double?)a.Litros) ?? 0,
                    TotalCustoEuros = _context.Abastecimentos.Where(a => a.Data.Year == data.Year && a.Data.Month == data.Month).Sum(a => (double?)a.CustoTotal) ?? 0,
                    MediaConsumoNoMes = _context.Abastecimentos.Where(a => a.Data.Year == data.Year && a.Data.Month == data.Month && a.ConsumoMedio != null).Average(a => (double?)a.ConsumoMedio) ?? 0
                })
                .OrderBy(x => x.AnoMes)
                .ToList();

            return new
            {
                TotalLitrosAcumulados = Math.Round(totalLitros, 2),
                CustoGlobalEuros = Math.Round(totalGasto, 2),
                MediaGeralFrotaL100Km = Math.Round(consumoMedioFrota, 2),
                DadosGraficoEvolucao = historicoConsumoGrafico
            };
        }
    }
}