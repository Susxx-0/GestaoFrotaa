using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Abastecimentos
{
    public class AbastecimentoService
    {
        private readonly AppDbContext _context;

        public AbastecimentoService(AppDbContext context)
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

        public async Task<Abastecimento> CriarAsync(Abastecimento abastecimento)
        {
            var veiculo = await _context.Veiculos.FindAsync(abastecimento.VeiculoId);
            if (veiculo == null)
                throw new InvalidOperationException("Veículo não encontrado.");

            abastecimento.Data = DateTime.Now;
            abastecimento.CustoTotal = abastecimento.Litros * abastecimento.PrecoPorLitro;
            abastecimento.ConsumoMedio = abastecimento.KmAtual > 0 && abastecimento.Litros > 0
                ? Math.Round(abastecimento.KmAtual / (double)abastecimento.Litros, 2)
                : 0;

            _context.Abastecimentos.Add(abastecimento);
            await _context.SaveChangesAsync();

            return abastecimento;
        }

        public async Task<Abastecimento?> AtualizarAsync(int id, Abastecimento dados)
        {
            var abastecimento = await _context.Abastecimentos.FindAsync(id);
            if (abastecimento == null)
                return null;

            abastecimento.Litros = dados.Litros;
            abastecimento.PrecoPorLitro = dados.PrecoPorLitro;
            abastecimento.CustoTotal = dados.Litros * dados.PrecoPorLitro;
            abastecimento.Combustivel = dados.Combustivel;
            abastecimento.Posto = dados.Posto;
            abastecimento.KmAtual = dados.KmAtual;
            abastecimento.ConsumoMedio = dados.ConsumoMedio;
            abastecimento.Data = dados.Data;

            await _context.SaveChangesAsync();
            return abastecimento;
        }

        public async Task<bool> ApagarAsync(int id)
        {
            var abastecimento = await _context.Abastecimentos.FindAsync(id);
            if (abastecimento == null)
                return false;

            _context.Abastecimentos.Remove(abastecimento);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
