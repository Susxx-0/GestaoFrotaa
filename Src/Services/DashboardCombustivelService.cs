using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services
{
    public class DashboardCombustivelService
    {
        private readonly AppDbContext _context;

        public DashboardCombustivelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterEstatisticasAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            // 🔹 Totais gerais
            var totalLitros = await _context.Abastecimentos.SumAsync(a => (double?)a.Litros) ?? 0;
            var totalGasto = await _context.Abastecimentos.SumAsync(a => (double?)a.CustoTotal) ?? 0;
            var totalGastoMes = await _context.Abastecimentos
                .Where(a => a.Data >= inicioMes)
                .SumAsync(a => (double?)a.CustoTotal) ?? 0;

            // 🔹 Consumo médio da frota
            var consumoMedioFrota = await _context.Abastecimentos
                .Where(a => a.ConsumoMedio != null)
                .AverageAsync(a => (double?)a.ConsumoMedio) ?? 0;

            // 🔹 Top 5 veículos mais gastadores
            var topGastadores = await _context.Abastecimentos
                .Include(a => a.Veiculo)
                .GroupBy(a => new { a.VeiculoId, a.Veiculo.Marca, a.Veiculo.Modelo })
                .Select(g => new
                {
                    g.Key.VeiculoId,
                    g.Key.Marca,
                    g.Key.Modelo,
                    TotalGasto = g.Sum(x => x.CustoTotal)
                })
                .OrderByDescending(x => x.TotalGasto)
                .Take(5)
                .ToListAsync();

            // 🔹 Últimos 6 meses — litros e custos
            var ultimos6Meses = Enumerable.Range(0, 6)
                .Select(i => hoje.AddMonths(-i))
                .Select(data => new
                {
                    Mes = data.ToString("yyyy-MM"),
                    Litros = _context.Abastecimentos
                        .Where(a => a.Data.Year == data.Year && a.Data.Month == data.Month)
                        .Sum(a => (double?)a.Litros) ?? 0,
                    Custo = _context.Abastecimentos
                        .Where(a => a.Data.Year == data.Year && a.Data.Month == data.Month)
                        .Sum(a => (double?)a.CustoTotal) ?? 0
                })
                .OrderBy(x => x.Mes)
                .ToList();

            return new
            {
                TotalLitros = Math.Round(totalLitros, 2),
                TotalGasto = Math.Round(totalGasto, 2),
                TotalGastoMes = Math.Round(totalGastoMes, 2),
                ConsumoMedioFrota = Math.Round(consumoMedioFrota, 2),
                TopGastadores = topGastadores,
                Ultimos6Meses = ultimos6Meses
            };
        }
    }
}
