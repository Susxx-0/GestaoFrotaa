using GestaoDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeFrotas.Services
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

            // 🔹 KM percorridos por veículo (baseado nos abastecimentos)
            var kmPorVeiculo = await _context.Abastecimentos
                .Include(a => a.Veiculo)
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

            // 🔹 Top 5 mais usados
            var topMaisUsados = kmPorVeiculo
                .OrderByDescending(x => x.KmPercorridos)
                .Take(5)
                .ToList();

            // 🔹 Top 5 menos usados
            var topMenosUsados = kmPorVeiculo
                .OrderBy(x => x.KmPercorridos)
                .Take(5)
                .ToList();

            // 🔹 KM percorridos no mês atual
            var kmMes = await _context.Abastecimentos
                .Where(a => a.Data >= inicioMes)
                .GroupBy(a => a.VeiculoId)
                .Select(g => g.Max(x => x.KmAtual) - g.Min(x => x.KmAtual))
                .SumAsync();

            // 🔹 Evolução dos últimos 6 meses
            var ultimos6Meses = Enumerable.Range(0, 6)
                .Select(i => hoje.AddMonths(-i))
                .Select(data => new
                {
                    Mes = data.ToString("yyyy-MM"),
                    KmPercorridos = _context.Abastecimentos
                        .Where(a => a.Data.Year == data.Year && a.Data.Month == data.Month)
                        .GroupBy(a => a.VeiculoId)
                        .Select(g => g.Max(x => x.KmAtual) - g.Min(x => x.KmAtual))
                        .Sum()
                })
                .OrderBy(x => x.Mes)
                .ToList();

            return new
            {
                KmPorVeiculo = kmPorVeiculo,
                TopMaisUsados = topMaisUsados,
                TopMenosUsados = topMenosUsados,
                KmPercorridosMesAtual = kmMes,
                Ultimos6Meses = ultimos6Meses
            };
        }
    }
}
