using GestaoDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeFrotas.Services
{
    public class DashboardManutencaoService
    {
        private readonly AppDbContext _context;

        public DashboardManutencaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterEstadoManutencoesAsync()
        {
            var veiculos = await _context.Veiculos.ToListAsync();
            var hoje = DateTime.Now;

            var lista = new List<object>();

            foreach (var v in veiculos)
            {
                if (v.IntervaloManutencaoKm == 0)
                {
                    lista.Add(new
                    {
                        VeiculoId = v.Id,
                        v.Marca,
                        v.Modelo,
                        v.Matricula,
                        Estado = "Sem Intervalo Definido",
                        Percentagem = 0,
                        KmDesdeUltima = 0,
                        ProximaManutencaoKm = 0
                    });
                    continue;
                }

                int kmDesdeUltima = v.UltimaManutencaoKm.HasValue
                    ? v.KmAtual - v.UltimaManutencaoKm.Value
                    : v.KmAtual;

                double percent = (double)kmDesdeUltima / v.IntervaloManutencaoKm * 100;

                string estado =
                    percent >= 120 ? "Atrasada" :
                    percent >= 90 ? "Próxima" :
                    "OK";

                lista.Add(new
                {
                    VeiculoId = v.Id,
                    v.Marca,
                    v.Modelo,
                    v.Matricula,
                    Estado = estado,
                    Percentagem = Math.Round(percent, 1),
                    KmDesdeUltima = kmDesdeUltima,
                    ProximaManutencaoKm = v.IntervaloManutencaoKm - kmDesdeUltima
                });
            }

            return new
            {
                Total = lista.Count,
                Ok = lista.Count(x => ((dynamic)x).Estado == "OK"),
                Proxima = lista.Count(x => ((dynamic)x).Estado == "Próxima"),
                Atrasada = lista.Count(x => ((dynamic)x).Estado == "Atrasada"),
                Veiculos = lista
            };
        }
    }
}
