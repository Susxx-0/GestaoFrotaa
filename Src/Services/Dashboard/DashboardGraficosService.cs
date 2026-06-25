using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Dashboard
{
    public class DashboardGraficosService
    {
        private readonly AppDbContext _context;

        public DashboardGraficosService(AppDbContext context)
        {
            _context = context;
        }

        //  CONSUMO MENSAL (LITROS E CUSTO)
        public async Task<object> ObterConsumoMensalAsync()
        {
            var dados = await _context.Abastecimentos
                .GroupBy(a => new { a.Data.Year, a.Data.Month })
                .Select(g => new
                {
                    Ano = g.Key.Year,
                    Mes = g.Key.Month,
                    Litros = g.Sum(x => x.Litros),
                    Custo = g.Sum(x => x.CustoTotal)
                })
                .OrderBy(x => x.Ano).ThenBy(x => x.Mes)
                .ToListAsync();

            return dados;
        }

        // VIAGENS POR MÊS
        public async Task<object> ObterViagensMensaisAsync()
        {
            var dados = await _context.Viagens
                .GroupBy(v => new { v.DataInicio.Year, v.DataInicio.Month })
                .Select(g => new
                {
                    Ano = g.Key.Year,
                    Mes = g.Key.Month,
                    TotalViagens = g.Count(),
                    ViagensAtivas = g.Count(v => v.EstaAtiva),
                    ViagensConcluidas = g.Count(v => v.DataFim != null)
                })
                .OrderBy(x => x.Ano).ThenBy(x => x.Mes)
                .ToListAsync();

            return dados;
        }
    }
}
