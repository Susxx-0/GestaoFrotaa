using GestoreDeFrotas.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogsPesquisaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogsPesquisaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("pesquisa")]
        public async Task<IActionResult> Pesquisar(
            [FromQuery] string? q,
            [FromQuery] string? utilizador,
            [FromQuery] string? metodo,
            [FromQuery] string? rota,
            [FromQuery] int? statusMin,
            [FromQuery] int? statusMax,
            [FromQuery] DateTime? dataMin,
            [FromQuery] DateTime? dataMax,
            [FromQuery] string? sort = "data,desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.LogsSistema.AsQueryable();

            // 🔍 Pesquisa global
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(l =>
                    (l.Utilizador != null && l.Utilizador.Contains(q)) ||
                    l.MetodoHttp.Contains(q) ||
                    l.Rota.Contains(q) ||
                    l.Descricao.Contains(q));
            }

            // 🎯 Filtros específicos
            if (!string.IsNullOrWhiteSpace(utilizador))
                query = query.Where(l => l.Utilizador != null && l.Utilizador.Contains(utilizador));

            if (!string.IsNullOrWhiteSpace(metodo))
                query = query.Where(l => l.MetodoHttp.Contains(metodo));

            if (!string.IsNullOrWhiteSpace(rota))
                query = query.Where(l => l.Rota.Contains(rota));

            if (statusMin.HasValue)
                query = query.Where(l => l.StatusCode >= statusMin.Value);

            if (statusMax.HasValue)
                query = query.Where(l => l.StatusCode <= statusMax.Value);

            if (dataMin.HasValue)
                query = query.Where(l => l.Data >= dataMin.Value);

            if (dataMax.HasValue)
                query = query.Where(l => l.Data <= dataMax.Value);

            // 📊 Total
            var total = await query.CountAsync();

            // ↕ Ordenação
            query = ApplyLogsSorting(query, sort);

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

        private IQueryable<Models.LogSistema> ApplyLogsSorting(IQueryable<Models.LogSistema> query, string? sort)
        {
            var sortField = "data";
            var sortDir = "desc";

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var parts = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length >= 1) sortField = parts[0].ToLower();
                if (parts.Length >= 2) sortDir = parts[1].ToLower();
            }

            bool desc = sortDir == "desc";

            return (sortField) switch
            {
                "statuscode" => desc ? query.OrderByDescending(l => l.StatusCode).ThenByDescending(l => l.Data)
                                     : query.OrderBy(l => l.StatusCode).ThenByDescending(l => l.Data),
                "metodohttp" => desc ? query.OrderByDescending(l => l.MetodoHttp).ThenByDescending(l => l.Data)
                                     : query.OrderBy(l => l.MetodoHttp).ThenByDescending(l => l.Data),
                "rota" => desc ? query.OrderByDescending(l => l.Rota).ThenByDescending(l => l.Data)
                               : query.OrderBy(l => l.Rota).ThenByDescending(l => l.Data),
                _ => desc ? query.OrderByDescending(l => l.Data)
                          : query.OrderBy(l => l.Data),
            };  
        }
    }
}
