using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class ManutencaoService
    {
        private readonly AppDbContext _context;
        public ManutencaoService(AppDbContext context) => _context = context;

        public async Task<List<RegistoManutencao>> ObterTodosAsync() =>
            await _context.RegistosManutencao.ToListAsync();

        public async Task<object?> ObterEstadoManutencaoAsync(int veiculoId)
        {
            var v = await _context.Veiculos.FindAsync(veiculoId);
            if (v == null) return null;

            return new
            {
                v.Id,
                v.Matricula,
                v.Estado,
                PrecisaManutencao = v.KmAtual >= v.ProximaManutencaoKm
            };
        }

        public async Task<bool> AdicionarAsync(RegistoManutencao reg)
        {
            _context.RegistosManutencao.Add(reg);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
