using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Dashboard
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
                (v.ProximaManutencaoKm.HasValue && v.KmAtual >= v.ProximaManutencaoKm) ||
                (v.ProximaManutencaoData.HasValue && v.ProximaManutencaoData <= DateTime.Today)
            );

            int ipoExpira = veiculos.Count(v =>
                v.DataProximaIpo.HasValue &&
                v.DataProximaIpo <= DateTime.Today.AddDays(7)
            );

            int seguroExpira = veiculos.Count(v =>
                v.DataSeguro.HasValue &&
                v.DataSeguro <= DateTime.Today.AddDays(7)
            );

            int inspecaoExpira = veiculos.Count(v =>
                v.DataInspecao.HasValue &&
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
