using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Veiculos
{
    public class AbastecimentosService
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<string, int> _limitesPorMarca = new()
        {
            { "BMW", 95 }, { "Mercedes", 90 }, { "Audi", 85 }, { "Volkswagen", 80 },
            { "Renault", 70 }, { "Peugeot", 70 }, { "Ford", 80 }, { "Toyota", 85 },
            { "Volvo", 85 }, { "Tesla", 0 }
        };

        private const int LimiteGeral = 100;

        public AbastecimentosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Abastecimento>> ObterTodosAsync()
        {
            return await _context.Abastecimentos
                .Include(a => a.Veiculo)
                .OrderByDescending(a => a.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<Abastecimento>> ObterPorVeiculoAsync(int veiculoId)
        {
            return await _context.Abastecimentos
                .Where(a => a.VeiculoId == veiculoId)
                .OrderByDescending(a => a.Data)
                .ToListAsync();
        }

        public async Task<Abastecimento?> ObterPorIdAsync(int id)
        {
            return await _context.Abastecimentos
                .Include(a => a.Veiculo)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Abastecimento> CriarAsync(Abastecimento ab)
        {
            var veiculo = await _context.Veiculos.FindAsync(ab.VeiculoId);
            if (veiculo == null)
                throw new Exception("Veículo não encontrado.");

            int limiteLitros = _limitesPorMarca.ContainsKey(veiculo.Marca)
                ? _limitesPorMarca[veiculo.Marca]
                : LimiteGeral;

            if (ab.Litros > limiteLitros)
                throw new Exception($"A marca {veiculo.Marca} permite no máximo {limiteLitros} litros por abastecimento.");

            ab.CustoTotal = Math.Round(ab.Litros * ab.PrecoPorLitro, 2);

            var ultimo = await _context.Abastecimentos
                .Where(a => a.VeiculoId == ab.VeiculoId)
                .OrderByDescending(a => a.Data)
                .FirstOrDefaultAsync();

            int kmPercorridos = 0;

            if (ultimo != null)
            {
                if (ab.KmAtual < ultimo.KmAtual)
                    throw new Exception("O KM atual não pode ser inferior ao do último registo.");

                kmPercorridos = ab.KmAtual - ultimo.KmAtual;
            }

            if (kmPercorridos > 0)
            {
                ab.KmPorLitro = Math.Round((double)kmPercorridos / ab.Litros, 2);
                ab.ConsumoMedio = Math.Round((ab.Litros / kmPercorridos) * 100, 2);
            }
            else
            {
                ab.KmPorLitro = 0;
                ab.ConsumoMedio = 0;
            }

            veiculo.KmAtual = ab.KmAtual;

            _context.Abastecimentos.Add(ab);
            await _context.SaveChangesAsync();

            return ab;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var ab = await _context.Abastecimentos.FindAsync(id);
            if (ab == null)
                return false;

            _context.Abastecimentos.Remove(ab);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
