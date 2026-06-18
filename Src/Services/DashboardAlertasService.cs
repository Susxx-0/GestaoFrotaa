using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services
{
    public class DashboardAlertasService
    {
        private readonly AppDbContext _context;

        public DashboardAlertasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterAlertasAsync()
        {
            var veiculos = await _context.Veiculos.ToListAsync();

            int manutencaoAtrasada = veiculos.Count(v =>
            {
                int kmProx = v.ProximaManutencaoKm ?? 0;
                return (kmProx > 0 && v.KmAtual >= kmProx)
                       || (v.ProximaManutencaoData != null && v.ProximaManutencaoData <= DateTime.Today);
            });

            int ipoExpira = veiculos.Count(v =>
                v.DataProximaIpo != null &&
                v.DataProximaIpo <= DateTime.Today.AddDays(7)
            );

            int seguroExpira = veiculos.Count(v =>
                v.DataSeguro != null &&
                v.DataSeguro <= DateTime.Today.AddDays(7)
            );

            int inspecaoExpira = veiculos.Count(v =>
                v.DataInspecao != null &&
                v.DataInspecao <= DateTime.Today.AddDays(7)
            );

            return new
            {
                ManutencaoAtrasada = manutencaoAtrasada,
                IPOExpira = ipoExpira,
                SeguroExpira = seguroExpira,
                InspecaoExpira = inspecaoExpira
            };
        }
    }
}
