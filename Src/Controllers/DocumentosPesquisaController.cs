using GestoreDeFrotas.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestoreDeFrotas.Models;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    [Authorize]
    public class DocumentosPesquisaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentosPesquisaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("pesquisa")]
        public async Task<IActionResult> Pesquisar(
            [FromQuery] string? q,
            [FromQuery] string? tipoDocumento,
            [FromQuery] int? veiculoId,
            [FromQuery] bool? expirado,
            [FromQuery] DateTime? dataMin,
            [FromQuery] DateTime? dataMax,
            [FromQuery] string? sort = "dataValidade,asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var hoje = DateTime.Now;
            var query = _context.DocumentosVeiculos.AsQueryable();

            //  Pesquisa global
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(d =>
                    d.TipoDocumento.Contains(q) ||
                    d.NomeFicheiroOriginal.Contains(q) ||
                    d.CaminhoFicheiro.Contains(q));
            }

            //  Filtros específicos
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

            //  Total
            var total = await query.CountAsync();

            //  Ordenação
            query = ApplyDocumentosSorting(query, sort);

            //  Paginação
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            return Ok(new
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        
        private IQueryable<DocumentoVeiculo> ApplyDocumentosSorting(IQueryable<DocumentoVeiculo> query, string? sort)
        {
            var sortField = "datavalidade";
            var sortDir = "asc";

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var parts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length >= 1) sortField = parts[0].ToLower();
                if (parts.Length >= 2) sortDir = parts[1].ToLower();
            }

            bool desc = sortDir == "desc";

            return (sortField) switch
            {
                "tipodocumento" => desc ? query.OrderByDescending(d => d.TipoDocumento)
                                        : query.OrderBy(d => d.TipoDocumento),
                "dataupload" => desc ? query.OrderByDescending(d => d.DataUpload)
                                     : query.OrderBy(d => d.DataUpload),
                "datavalidade" => desc ? query.OrderByDescending(d => d.DataValidade)
                                       : query.OrderBy(d => d.DataValidade),
                _ => desc ? query.OrderByDescending(d => d.DataValidade).ThenBy(d => d.TipoDocumento)
                          : query.OrderBy(d => d.DataValidade).ThenBy(d => d.TipoDocumento),
            };
        }
    }
}