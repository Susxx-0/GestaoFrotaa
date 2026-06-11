using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GestoreDeFrotas.Services
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
            var seisMesesAtras = hoje.AddMonths(-6);

            // 1. KM percorridos por veículo (Optimizado com projeção e AsNoTracking)
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

            // 2. KM percorridos no mês atual (Consulta única eficiente)
            var kmMes = await _context.Abastecimentos
                .AsNoTracking()
                .Where(a => a.Data >= inicioMes)
                .GroupBy(a => a.VeiculoId)
                .Select(g => g.Max(x => x.KmAtual) - g.Min(x => x.KmAtual))
                .SumAsync();

            // 3. Evolução dos últimos 6 meses (Eliminamos o N+1)
            // Agrupamos tudo em uma única consulta ao banco
            var dadosEvolucao = await _context.Abastecimentos
                .AsNoTracking()
                .Where(a => a.Data >= seisMesesAtras)
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
                TopMaisUsados = kmPorVeiculo.Take(5).ToList(),
                TopMenosUsados = kmPorVeiculo.OrderBy(x => x.KmPercorridos).Take(5).ToList(),
                KmPercorridosMesAtual = kmMes,
                Ultimos6Meses = dadosEvolucao
            };
        }
    }
}