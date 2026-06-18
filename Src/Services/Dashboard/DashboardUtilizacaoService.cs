using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Dashboard
{
    public class DashboardUtilizacaoService
    {
        private readonly AppDbContext _context;

        public DashboardUtilizacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterEstatisticasAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var seisMeses = hoje.AddMonths(-6);

            var kmPorVeiculo = await _context.Abastecimentos
                .AsNoTracking()
                .GroupBy(a => new { a.VeiculoId, a.Veiculo.Marca, a.Veiculo.Modelo })
                .Select(g => new
                {
                    g.Key.VeiculoId,
                    g.Key.Marca,
                    g.Key.Modelo,
                    KmPercorridos = g.Max(x => x.KmAtual) - g.Min(x => x.KmAtual)
                })
                .OrderByDescending(x => x.KmPercorridos)
                .ToListAsync();

            var kmMes = await _context.Abastecimentos
                .AsNoTracking()
                .Where(a => a.Data >= inicioMes)
                .GroupBy(a => a.VeiculoId)
                .Select(g => g.Max(x => x.KmAtual) - g.Min(x => x.KmAtual))
                .SumAsync();

            var evolucao = await _context.Abastecimentos
                .AsNoTracking()
                .Where(a => a.Data >= seisMeses)
                .GroupBy(a => new { a.Data.Year, a.Data.Month })
                .Select(g => new
                {
                    Mes = $"{g.Key.Year}-{g.Key.Month:D2}",
                    KmPercorridos = g.Max(x => x.KmAtual) - g.Min(x => x.KmAtual)
                })
                .OrderBy(x => x.Mes)
                .ToListAsync();

            return new
            {
                KmPorVeiculo = kmPorVeiculo,
                TopMaisUsados = kmPorVeiculo.Take(5),
                TopMenosUsados = kmPorVeiculo.OrderBy(x => x.KmPercorridos).Take(5),
                KmPercorridosMesAtual = kmMes,
                Ultimos6Meses = evolucao
            };
        }
    }
}
