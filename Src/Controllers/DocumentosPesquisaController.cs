using GestoreDeFrotas.Services.Documentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    [Authorize]
    public class DocumentosPesquisaController : ControllerBase
    {
        private readonly DocumentosPesquisaService _service;

        public DocumentosPesquisaController(DocumentosPesquisaService service)
        {
            _service = service;
        }

        [HttpGet("pesquisa")]
        public async Task<IActionResult> Pesquisar(
            [FromQuery] string? q,
            [FromQuery] string? tipoDocumento,
            [FromQuery] int? veiculoId,
            [FromQuery] bool? expirado,
            [FromQuery] DateTime? dataMin,
            [FromQuery] DateTime? dataMax,
            [FromQuery] string? sort = "datavalidade,asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _service.PesquisarAsync(
                q, tipoDocumento, veiculoId, expirado, dataMin, dataMax, sort, page, pageSize
            );

            return Ok(new
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }
    }
}
