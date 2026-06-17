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

        public async Task<object> ObterEstatisticasConsumoAsync()
        {
            var hoje = DateTime.Now;

            // Genera la estructura idónea para las Gráficas Lineales de consumo a lo largo del tiempo (6 meses)
            var dadosGrafico = Enumerable.Range(0, 6)
                .Select(i => hoje.AddMonths(-i))
                .Select(d => new {
                    AnoMes = d.ToString("yyyy-MM"),
                    TotalLitros = _context.Abastecimentos.Where(a => a.Data.Year == d.Year && a.Data.Month == d.Month).Sum(a => (double?)a.Litros) ?? 0,
                    TotalGastoEuro = _context.Abastecimentos.Where(a => a.Data.Year == d.Year && a.Data.Month == d.Month).Sum(a => (double?)a.CustoTotal) ?? 0
                }).OrderBy(x => x.AnoMes).ToList();

            return new
            {
                MediaGeralFrotaL100Km = await _context.Abastecimentos.AverageAsync(a => (double?)a.ConsumoMedio) ?? 0,
                DadosGraficoEvolucao = dadosGrafico
            };
        }
    }
}