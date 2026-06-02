using GestaoDeFrotas.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/veiculos")]
    [Authorize]
    public class VeiculosPesquisaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeiculosPesquisaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("pesquisa")]
        public async Task<IActionResult> Pesquisar(
            [FromQuery] string? q,
            [FromQuery] string? marca,
            [FromQuery] string? modelo,
            [FromQuery] string? matricula,
            [FromQuery] string? estado,
            [FromQuery] string? cor,
            [FromQuery] int? anoMin,
            [FromQuery] int? anoMax,
            [FromQuery] string? sort = "marca,asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Veiculos.AsQueryable();

            // 🔍 Pesquisa global
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(v =>
                    v.Marca.Contains(q) ||
                    v.Modelo.Contains(q) ||
                    v.Matricula.Contains(q) ||
                    v.Cor.Contains(q) ||
                    v.Estado.Contains(q));
            }

            // 🎯 Filtros específicos
            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(v => v.Marca.Contains(marca));

            if (!string.IsNullOrWhiteSpace(modelo))
                query = query.Where(v => v.Modelo.Contains(modelo));

            if (!string.IsNullOrWhiteSpace(matricula))
                query = query.Where(v => v.Matricula.Contains(matricula));

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(v => v.Estado.Contains(estado));

            if (!string.IsNullOrWhiteSpace(cor))
                query = query.Where(v => v.Cor.Contains(cor));

            if (anoMin.HasValue)
                query = query.Where(v => v.Ano >= anoMin.Value);

            if (anoMax.HasValue)
                query = query.Where(v => v.Ano <= anoMax.Value);

            // 📊 Total antes da paginação
            var total = await query.CountAsync();

            // ↕ Ordenação
            query = ApplySorting(query, sort);

            // 📄 Paginação
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

        private IQueryable<Models.Veiculo> ApplySorting(IQueryable<Models.Veiculo> query, string? sort)
        {
            var sortField = "marca";
            var sortDir = "asc";

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var parts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length >= 1) sortField = parts[0].ToLower();
                if (parts.Length >= 2) sortDir = parts[1].ToLower();
            }

            bool desc = sortDir == "desc";

            return sortField switch
            {
                "modelo" => desc ? query.OrderByDescending(v => v.Modelo) : query.OrderBy(v => v.Modelo),
                "matricula" => desc ? query.OrderByDescending(v => v.Matricula) : query.OrderBy(v => v.Matricula),
                "ano" => desc ? query.OrderByDescending(v => v.Ano) : query.OrderBy(v => v.Ano),
                "estado" => desc ? query.OrderByDescending(v => v.Estado) : query.OrderBy(v => v.Estado),
                "cor" => desc ? query.OrderByDescending(v => v.Cor) : query.OrderBy(v => v.Cor),
                _ => desc ? query.OrderByDescending(v => v.Marca) : query.OrderBy(v => v.Marca),
            };
        }
    }
}
