using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Documentos
{
    public class DocumentosPesquisaService
    {
        private readonly AppDbContext _context;

        public DocumentosPesquisaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<DocumentoVeiculo> Items, int Total)> PesquisarAsync(
            string? q,
            string? tipoDocumento,
            int? veiculoId,
            bool? expirado,
            DateTime? dataMin,
            DateTime? dataMax,
            string? sort,
            int page,
            int pageSize)
        {
            var hoje = DateTime.Now;
            var query = _context.DocumentosVeiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(d =>
                    d.TipoDocumento.Contains(q) ||
                    d.NomeFicheiroOriginal.Contains(q) ||
                    d.CaminhoFicheiro.Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(tipoDocumento))
                query = query.Where(d => d.TipoDocumento.Contains(tipoDocumento));

            if (veiculoId.HasValue)
                query = query.Where(d => d.VeiculoId == veiculoId.Value);

            if (expirado.HasValue)
            {
                if (expirado.Value)
                    query = query.Where(d => d.DataValidade != null && d.DataValidade < hoje);
                else
                    query = query.Where(d => d.DataValidade == null || d.DataValidade >= hoje);
            }

            if (dataMin.HasValue)
                query = query.Where(d => d.DataUpload >= dataMin.Value);

            if (dataMax.HasValue)
                query = query.Where(d => d.DataUpload <= dataMax.Value);

            int total = await query.CountAsync();

            query = AplicarOrdenacao(query, sort);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        private IQueryable<DocumentoVeiculo> AplicarOrdenacao(IQueryable<DocumentoVeiculo> query, string? sort)
        {
            var campo = "datavalidade";
            var direcao = "asc";

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var partes = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (partes.Length >= 1) campo = partes[0].ToLower();
                if (partes.Length >= 2) direcao = partes[1].ToLower();
            }

            bool desc = direcao == "desc";

            return campo switch
            {
                "tipodocumento" => desc ? query.OrderByDescending(d => d.TipoDocumento) : query.OrderBy(d => d.TipoDocumento),
                "dataupload" => desc ? query.OrderByDescending(d => d.DataUpload) : query.OrderBy(d => d.DataUpload),
                "datavalidade" => desc ? query.OrderByDescending(d => d.DataValidade) : query.OrderBy(d => d.DataValidade),
                _ => desc ? query.OrderByDescending(d => d.DataValidade).ThenBy(d => d.TipoDocumento)
                          : query.OrderBy(d => d.DataValidade).ThenBy(d => d.TipoDocumento),
            };
        }
    }
}
