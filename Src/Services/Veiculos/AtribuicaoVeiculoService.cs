using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Veiculos
{
    public class AtribuicaoVeiculoService
    {
        private readonly AppDbContext _context;

        public AtribuicaoVeiculoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AtribuirAsync(int userId, int veiculoId)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null)
                return "Veículo não encontrado.";

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return "Utilizador não encontrado.";

            var ativo = await _context.AtribuicoesVeiculo
                .AnyAsync(a => a.VeiculoId == veiculoId && a.Ativo);

            if (ativo)
                return "O veículo já está atribuído.";

            var nova = new AtribuicaoVeiculo
            {
                UserId = userId,
                VeiculoId = veiculoId,
                DataInicio = DateTime.Now,
                Ativo = true
            };

            _context.AtribuicoesVeiculo.Add(nova);
            await _context.SaveChangesAsync();

            return "Veículo atribuído com sucesso.";
        }

        public async Task<string> RemoverAtribuicaoAsync(int veiculoId)
        {
            var atrib = await _context.AtribuicoesVeiculo
                .FirstOrDefaultAsync(a => a.VeiculoId == veiculoId && a.Ativo);

            if (atrib == null)
                return "O veículo não está atribuído.";

            atrib.Ativo = false;
            atrib.DataFim = DateTime.Now;

            await _context.SaveChangesAsync();
            return "Atribuição removida.";
        }

        public async Task<object?> ObterAtribuicaoAtualAsync(int veiculoId)
        {
            return await _context.AtribuicoesVeiculo
                .Include(a => a.User)
                .Where(a => a.VeiculoId == veiculoId && a.Ativo)
                .Select(a => new
                {
                    a.UserId,
                    a.User.Nome,
                    a.DataInicio
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<object>> ObterVeiculosDoUserAsync(int userId)
        {
            return await _context.AtribuicoesVeiculo
                .Include(a => a.Veiculo)
                .Where(a => a.UserId == userId && a.Ativo)
                .Select(a => new
                {
                    a.VeiculoId,
                    a.Veiculo.Marca,
                    a.Veiculo.Modelo,
                    a.Veiculo.Matricula,
                    a.DataInicio
                })
                .ToListAsync();
        }
    }
}
