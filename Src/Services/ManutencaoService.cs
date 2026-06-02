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
            _context.RegistosManutencao.Add(registo);
            await _context.SaveChangesAsync();
        }
    }
}