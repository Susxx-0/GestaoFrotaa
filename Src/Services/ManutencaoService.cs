using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class ManutencaoService
    {
        private readonly AppDbContext _context;
        public ManutencaoService(AppDbContext context) => _context = context;

        public async Task AdicionarAsync(RegistoManutencao m)
        {
            _context.Manutencoes.Add(m);
            var veiculo = await _context.Veiculos.FindAsync(m.VeiculoId);
            if (veiculo != null)
            {
                veiculo.UltimaManutencaoKm = m.Km;
                veiculo.UltimaManutencaoData = m.Data;
                veiculo.Estado = "Disponível";
            }
            await _context.SaveChangesAsync();
        }
    }
}