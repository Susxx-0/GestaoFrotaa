using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeFrotas.Services
{
    public class ManutencaoService
    {
        private readonly AppDbContext _context;

        public ManutencaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegistoManutencao>> ObterTodosAsync()
        {
            return await _context.RegistosManutencao.ToListAsync();
        }

        public async Task AdicionarAsync(RegistoManutencao registo)
        {
            var veiculo = await _context.Veiculos.FindAsync(registo.VeiculoId);
            if (veiculo == null)
                throw new Exception("Veículo não encontrado.");

            veiculo.UltimaManutencaoKm = registo.Km;
            veiculo.UltimaManutencaoData = registo.Data;

            _context.RegistosManutencao.Add(registo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> ObterEstadoManutencaoAsync()
        {
            var veiculos = await _context.Veiculos.ToListAsync();
            var lista = new List<object>();

            foreach (var v in veiculos)
            {
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
                    v.Id,
                    v.Marca,
                    v.Modelo,
                    v.Matricula,
                    KmDesdeUltima = kmDesdeUltima,
                    Percentagem = Math.Round(percent, 1),
                    Estado = estado
                });
            }

            return lista;
        }
    }
}
