using GestoreDeFrotas.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Controllers
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
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(v =>
                    v.Marca.Contains(q) ||
                    v.Modelo.Contains(q) ||
                    v.Matricula.Contains(q) ||
                    v.Cor.Contains(q) ||
                    v.Estado.Contains(q));
            }

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

            var total = await query.CountAsync();

            query = AplicarOrdenacao(query, sort);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }

        private IQueryable<Models.Veiculo> AplicarOrdenacao(IQueryable<Models.Veiculo> query, string? sort)
        {
            var campo = "marca";
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
