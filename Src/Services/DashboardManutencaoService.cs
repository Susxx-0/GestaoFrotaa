using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services
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

            int manutencaoAtrasada = veiculos.Count(v =>
            {
                int kmProx = v.ProximaManutencaoKm ?? 0;
                return (kmProx > 0 && v.KmAtual >= kmProx)
                       || (v.ProximaManutencaoData != null && v.ProximaManutencaoData <= DateTime.Today);
            });

            int manutencaoProxima = veiculos.Count(v =>
            {
                int kmProx = v.ProximaManutencaoKm ?? 0;
                return (kmProx > 0 && v.KmAtual >= kmProx - 1000)
                       || (v.ProximaManutencaoData != null && v.ProximaManutencaoData <= DateTime.Today.AddDays(30));
            });

            return new
            {
                Atrasada = manutencaoAtrasada,
                Proxima = manutencaoProxima
            };
        }
    }
}
