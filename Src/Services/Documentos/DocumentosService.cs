using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Documentos
{
    public class DocumentosService
    {
        private readonly AppDbContext _context;

        public DocumentosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DocumentoVeiculo>> ObterPorVeiculoAsync(int veiculoId)
        {
            return await _context.DocumentosVeiculos
                .Where(d => d.VeiculoId == veiculoId)
                .OrderByDescending(d => d.DataUpload)
                .ToListAsync();
        }

        public async Task<List<DocumentoVeiculo>> ObterExpiradosAsync()
        {
            var hoje = DateTime.Now;
            var limite = hoje.AddDays(30);

            return await _context.DocumentosVeiculos
                .Where(d => d.DataValidade != null && d.DataValidade <= limite)
                .OrderBy(d => d.DataValidade)
                .ToListAsync();
        }

        public async Task<DocumentoVeiculo?> ObterPorIdAsync(int id)
        {
            return await _context.DocumentosVeiculos.FindAsync(id);
        }

        public async Task<DocumentoVeiculo> CriarAsync(DocumentoVeiculo doc)
        {
            _context.DocumentosVeiculos.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var doc = await _context.DocumentosVeiculos.FindAsync(id);
            if (doc == null)
                return false;

            _context.DocumentosVeiculos.Remove(doc);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
