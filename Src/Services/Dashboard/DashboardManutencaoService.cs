using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Dashboard
{
    public class DashboardManutencaoService
    {
        private readonly AppDbContext _context;

        public DashboardManutencaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterResumoAsync()
        {
            var veiculos = await _context.Veiculos.ToListAsync();

            int atrasada = veiculos.Count(v =>
                (v.ProximaManutencaoKm.HasValue && v.KmAtual >= v.ProximaManutencaoKm.Value) ||
                (v.ProximaManutencaoData.HasValue && v.ProximaManutencaoData.Value <= DateTime.Today)
            );

            int proxima = veiculos.Count(v =>
                (v.ProximaManutencaoKm.HasValue && v.KmAtual >= v.ProximaManutencaoKm.Value - 1000) ||
                (v.ProximaManutencaoData.HasValue && v.ProximaManutencaoData.Value <= DateTime.Today.AddDays(30))
            );

            return new
            {
                Atrasada = atrasada,
                Proxima = proxima
            };
        }
    }
}
