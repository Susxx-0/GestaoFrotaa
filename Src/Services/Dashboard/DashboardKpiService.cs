using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Dashboard
{
    public class DashboardKpiService
    {
        private readonly AppDbContext _context;

        public DashboardKpiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterKpisAsync()
        {
            var hoje = DateTime.Today;
            var limite = hoje.AddDays(30);

            var totalVeiculos = await _context.Veiculos.CountAsync();
            var veiculosEmUso = await _context.Viagens.CountAsync(v => v.EstaAtiva);
            var veiculosDisponiveis = totalVeiculos - veiculosEmUso;

            var viagensAtivas = veiculosEmUso;
            var viagensHoje = await _context.Viagens.CountAsync(v => v.DataInicio.Date == hoje);

            var manutencoesPendentes = await _context.RegistosManutencao
                .CountAsync(m => m.DataConclusao == null);

            var cartasExpiram = await _context.Users
                .CountAsync(u => u.CartaConducaoValidade != null &&
                                 u.CartaConducaoValidade <= limite);

            var ipoExpira = await _context.Veiculos
                .CountAsync(v => v.DataProximaIpo != null &&
                                 v.DataProximaIpo <= limite);

            var segurosExpiram = await _context.Veiculos
                .CountAsync(v => v.DataSeguro != null &&
                                 v.DataSeguro <= limite);

            return new
            {
                totalVeiculos,
                veiculosEmUso,
                veiculosDisponiveis,
                viagensAtivas,
                viagensHoje,
                manutencoesPendentes,
                cartasExpiram,
                ipoExpira,
                segurosExpiram
            };
        }
    }
}
