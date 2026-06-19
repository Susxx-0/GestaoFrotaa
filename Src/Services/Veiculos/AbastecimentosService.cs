using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Models.Dtos.Abastecimentos;
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

        public async Task<Abastecimento?> ObterPorIdAsync(int id)
        {
            return await _context.Abastecimentos
                .Include(a => a.Veiculo)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Abastecimento> CriarAsync(AbastecimentoCreateDTO dto)
        {
            var veiculo = await _context.Veiculos.FindAsync(dto.VeiculoId);
            if (veiculo == null)
                throw new InvalidOperationException("Veículo não encontrado.");

            var abastecimento = new Abastecimento
            {
                VeiculoId = dto.VeiculoId,
                Litros = dto.Litros,
                PrecoPorLitro = dto.PrecoPorLitro,
                KmAtual = dto.KmAtual,
                Combustivel = dto.Combustivel,
                Posto = dto.Posto,
                Data = DateTime.Now,
                CustoTotal = dto.Litros * dto.PrecoPorLitro,
                ConsumoMedio = dto.KmAtual > 0 && dto.Litros > 0
                    ? Math.Round(dto.KmAtual / dto.Litros, 2)
                    : 0
            };

            _context.Abastecimentos.Add(abastecimento);
            await _context.SaveChangesAsync();

            return abastecimento;
        }

        public async Task<Abastecimento?> AtualizarAsync(int id, AbastecimentoUpdateDTO dto)
        {
            var abastecimento = await _context.Abastecimentos.FindAsync(id);
            if (abastecimento == null)
                return null;

            abastecimento.Litros = dto.Litros;
            abastecimento.PrecoPorLitro = dto.PrecoPorLitro;
            abastecimento.CustoTotal = dto.Litros * dto.PrecoPorLitro;
            abastecimento.Combustivel = dto.Combustivel;
            abastecimento.Posto = dto.Posto;
            abastecimento.KmAtual = dto.KmAtual;
            abastecimento.ConsumoMedio = dto.KmAtual > 0 && dto.Litros > 0
                ? Math.Round(dto.KmAtual / dto.Litros, 2)
                : 0;
            abastecimento.Data = dto.Data;

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
