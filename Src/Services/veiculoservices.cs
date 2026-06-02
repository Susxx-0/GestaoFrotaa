using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeFrotas.Services
{
    public class VeiculosService
    {
        private readonly AppDbContext _context;

        public VeiculosService(AppDbContext context)
        {
            _context = context;
        }

        // NOVO: FILTROS DE PESQUISA
        public async Task<IEnumerable<Veiculo>> FiltrarAsync(
            string? marca,
            string? modelo,
            string? matricula,
            string? estado,
            string? cor,
            int? anoMin,
            int? anoMax)
        {
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(v => v.Marca.Contains(marca));

            if (!string.IsNullOrWhiteSpace(modelo))
                query = query.Where(v => v.Modelo.Contains(modelo));

            if (!string.IsNullOrWhiteSpace(matricula))
                query = query.Where(v => v.Matricula.Contains(matricula));

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(v => v.Estado == estado);

            if (!string.IsNullOrWhiteSpace(cor))
                query = query.Where(v => v.Cor.Contains(cor));

            if (anoMin.HasValue)
                query = query.Where(v => v.Ano >= anoMin.Value);

            if (anoMax.HasValue)
                query = query.Where(v => v.Ano <= anoMax.Value);

            return await query.ToListAsync();
        }

        // MÉTODOS ANTIGOS (mantidos)
        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            return await _context.Veiculos.ToListAsync();
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id)
        {
            return await _context.Veiculos.FindAsync(id);
        }

        public async Task AdicionarAsync(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Veiculo veiculo)
        {
            _context.Entry(veiculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) return false;

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
