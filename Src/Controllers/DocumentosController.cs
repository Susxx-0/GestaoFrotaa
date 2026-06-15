using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    public class DocumentosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<DocumentoUploadDto> _validator; // Injeção do validador

        public DocumentosController(AppDbContext context, IValidator<DocumentoUploadDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")   ]
        public async Task<IActionResult> UploadDocumento([FromForm] DocumentoUploadDto dto)
        {
            // Executa a validação do FluentValidation
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // Segurança: validação da extensão do ficheiro
            var extensao = Path.GetExtension(dto.Ficheiro!.FileName).ToLower();
            var extensoesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest("Apenas são permitidos ficheiros em formato PDF, JPG, JPEG ou PNG por motivos de segurança.");
            }

            var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(pastaUploads))
            {
                Directory.CreateDirectory(pastaUploads);
            }

            var nomeFicheiroUnico = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Ficheiro.FileName)}";
            var caminhoCompleto = Path.Combine(pastaUploads, nomeFicheiroUnico);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await dto.Ficheiro.CopyToAsync(stream);
            }

            var novoDocumento = new DocumentoVeiculo
            {
                VeiculoId = dto.VeiculoId,
                TipoDocumento = dto.TipoDocumento,
                NomeFicheiroOriginal = dto.Ficheiro.FileName,
                CaminhoFicheiro = $"/uploads/{nomeFicheiroUnico}",
                DataValidade = dto.DataValidade
            };

            _context.DocumentosVeiculos.Add(novoDocumento);
            await _context.SaveChangesAsync();

            if (dto.DataValidade.HasValue && dto.DataValidade.Value < DateTime.Now)
            {
                var auditoriaService = HttpContext.RequestServices.GetService(typeof(GestoreDeFrotas.Services.AuditoriaService)) as GestoreDeFrotas.Services.AuditoriaService;
                if (auditoriaService != null)
                {
                    await auditoriaService.CriarNotificacaoAsync(
   
                        "Documento Caducado Detetado",
   
                        $"Foi enviado um documento ({dto.TipoDocumento}) já expirado para o veículo com o ID {dto.VeiculoId}.",
    
                        "Warning"
);
                }
            }

            return Ok(novoDocumento);
        }

        [HttpGet("expirados")]
        public async Task<IActionResult> ObterDocumentosExpirados()
        {
            var hoje = DateTime.Now;
            var daquiA30Dias = hoje.AddDays(30);

            var expiradosOuQuase = await _context.DocumentosVeiculos
                .Where(d => d.DataValidade != null && d.DataValidade <= daquiA30Dias)
                .OrderBy(d => d.DataValidade)
                .ToListAsync();

            return Ok(expiradosOuQuase);
        }

        [HttpGet("veiculo/{veiculoId}")]
        public async Task<IActionResult> ObterDocumentosPorVeiculo(int veiculoId)
        {
            var documentos = await _context.DocumentosVeiculos
                .Where(d => d.VeiculoId == veiculoId)
                .OrderByDescending(d => d.DataUpload)
                .ToListAsync();

            return Ok(documentos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarDocumento(int id)
        {
            var documento = await _context.DocumentosVeiculos.FindAsync(id);
            if (documento == null) return NotFound("Documento não encontrado.");

            // CORRIGIDO: Alterado de 'documento.Caminho' para 'documento.CaminhoFicheiro.TrimStart('/')'
            var caminhoFisico = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", documento.CaminhoFicheiro.TrimStart('/'));

            if (System.IO.File.Exists(caminhoFisico))
            {
                System.IO.File.Delete(caminhoFisico);
            }

            _context.DocumentosVeiculos.Remove(documento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DescarregarDocumento(int id)
        {
            var doc = await _context.DocumentosVeiculos.FindAsync(id);
            if (doc == null) return NotFound("Documento não encontrado.");

            var caminhoFisico = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.CaminhoFicheiro.TrimStart('/'));
            if (!System.IO.File.Exists(caminhoFisico)) return NotFound("Ficheiro físico não encontrado no servidor.");

            var bytes = await System.IO.File.ReadAllBytesAsync(caminhoFisico);

            return File(bytes, "application/octet-stream", doc.NomeFicheiroOriginal);
        }
    }
}