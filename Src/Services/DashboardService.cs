using GestaoDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeFrotas.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterResumoGeralAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var totalVeiculos = await _context.Veiculos.CountAsync();

            var resumo = new
            {
                TotalVeiculos = totalVeiculos,
                Disponiveis = await _context.Veiculos.CountAsync(v => v.Estado == "Disponível"),
                Alugados = await _context.Veiculos.CountAsync(v => v.Estado == "Alugado"),
                EmManutencao = await _context.Veiculos.CountAsync(v => v.Estado == "Em Manutenção"),

                TotalAbastecimentos = await _context.Abastecimentos.CountAsync(),
                TotalGastoCombustivel = await _context.Abastecimentos.SumAsync(a => (double?)a.CustoTotal) ?? 0,
                TotalGastoCombustivelMes = await _context.Abastecimentos
                    .Where(a => a.Data >= inicioMes)
                    .SumAsync(a => (double?)a.CustoTotal) ?? 0,

                NumeroTotalManutencoes = await _context.RegistosManutencao.CountAsync(),
                TotalGastoManutencoes = await _context.RegistosManutencao.SumAsync(m => (decimal?)m.Custo) ?? 0,
                TotalGastoManutencoesMes = await _context.RegistosManutencao
                    .Where(m => m.Data >= inicioMes)
                    .SumAsync(m => (decimal?)m.Custo) ?? 0,

                TotalDocumentos = await _context.DocumentosVeiculos.CountAsync(),
                DocumentosExpirados = await _context.DocumentosVeiculos
                    .CountAsync(d => d.DataValidade != null && d.DataValidade < hoje),

                MediaKmAtual = await _context.Veiculos.AnyAsync()
                    ? await _context.Veiculos.AverageAsync(v => (double?)v.KmAtual) ?? 0
                    : 0
            };

            return resumo;
        }
    }
}
