using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Services
{
    public class AbastecimentosService
    {
        private readonly AppDbContext _context;

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

        public async Task<Abastecimento> AdicionarAsync(Abastecimento ab)
        {
            // Cálculo automático do custo total
            ab.CustoTotal = Math.Round(ab.Litros * ab.PrecoPorLitro, 2);

            // Buscar último abastecimento do veículo para calcular médias
            var ultimo = await _context.Abastecimentos
                .Where(a => a.VeiculoId == ab.VeiculoId)
                .OrderByDescending(a => a.Data)
                .FirstOrDefaultAsync();

            if (ultimo != null)
            {
                if (ab.KmAtual < ultimo.KmAtual)
                    throw new Exception("O KM atual não pode ser inferior ao do último abastecimento.");

                int kmPercorridos = ab.KmAtual - ultimo.KmAtual;

                if (kmPercorridos > 0)
                {
                    ab.KmPorLitro = Math.Round(kmPercorridos / ab.Litros, 2);
                    ab.ConsumoMedio = Math.Round((ab.Litros / kmPercorridos) * 100, 2);
                }
            }

            _context.Abastecimentos.Add(ab);
            await _context.SaveChangesAsync();
            return ab;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var ab = await _context.Abastecimentos.FindAsync(id);
            if (ab == null) return false;

            _context.Abastecimentos.Remove(ab);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}