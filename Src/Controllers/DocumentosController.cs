using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services.Documentos;
using GestoreDeFrotas.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    [Authorize]
    public class DocumentosController : ControllerBase
    {
        private readonly DocumentosService _service;
        private readonly IValidator<DocumentoUploadDto> _validator;

        public DocumentosController(DocumentosService service, IValidator<DocumentoUploadDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] DocumentoUploadDto dto)
        {
            var valid = await _validator.ValidateAsync(dto);
            if (!valid.IsValid)
                return BadRequest(valid.Errors.Select(e => e.ErrorMessage));

            var ext = Path.GetExtension(dto.Ficheiro!.FileName).ToLower();
            var permitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            if (!permitidas.Contains(ext))
                return BadRequest("Apenas PDF, JPG, JPEG ou PNG são permitidos.");

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var nome = $"{Guid.NewGuid()}_{dto.Ficheiro.FileName}";
            var caminho = Path.Combine(pasta, nome);

            using (var stream = new FileStream(caminho, FileMode.Create))
                await dto.Ficheiro.CopyToAsync(stream);

            var doc = new DocumentoVeiculo
            {
                VeiculoId = dto.VeiculoId,
                TipoDocumento = dto.TipoDocumento,
                NomeFicheiroOriginal = dto.Ficheiro.FileName,
                CaminhoFicheiro = $"/uploads/{nome}",
                DataValidade = dto.DataValidade
            };

            await _service.CriarAsync(doc);

            return Ok(doc);
        }

        [HttpGet("expirados")]
        public async Task<IActionResult> Expirados()
        {
            return Ok(await _service.ObterExpiradosAsync());
        }

        [HttpGet("veiculo/{veiculoId}")]
        public async Task<IActionResult> PorVeiculo(int veiculoId)
        {
            return Ok(await _service.ObterPorVeiculoAsync(veiculoId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Apagar(int id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var doc = await _service.ObterPorIdAsync(id);
            if (doc == null) return NotFound();

            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.CaminhoFicheiro.TrimStart('/'));

            if (!System.IO.File.Exists(caminho))
                return NotFound("Ficheiro não encontrado.");

            var bytes = await System.IO.File.ReadAllBytesAsync(caminho);

            return File(bytes, "application/octet-stream", doc.NomeFicheiroOriginal);
        }
    }
}
