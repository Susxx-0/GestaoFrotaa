using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    public class DocumentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocumentosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocumento(
            [FromForm] int veiculoId,
            [FromForm] string tipoDocumento,
            [FromForm] DateTime? dataValidade,
            IFormFile ficheiro)
        {
            if (ficheiro == null || ficheiro.Length == 0)
                return BadRequest("Nenhum ficheiro foi enviado.");

            // ---- IDEIA 1: VALIDAÇÃO DE SEGURANÇA (EXTENSÕES) ----
            var extensao = Path.GetExtension(ficheiro.FileName).ToLower();
            var extensoesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest("Apenas são permitidos ficheiros em formato PDF, JPG, JPEG ou PNG por motivos de segurança.");
            }

            // Criar a pasta 'wwwroot/uploads' caso ela não exista
            var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(pastaUploads))
            {
                Directory.CreateDirectory(pastaUploads);
            }

            // Gerar um nome único para o ficheiro não ser sobreposto
            var nomeFicheiroUnico = $"{Guid.NewGuid()}_{Path.GetFileName(ficheiro.FileName)}";
            var caminhoCompleto = Path.Combine(pastaUploads, nomeFicheiroUnico);

            // Guardar o ficheiro fisicamente na pasta
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await ficheiro.CopyToAsync(stream);
            }

            // Guardar o registo com o link na Base de Dados
            var novoDocumento = new DocumentoVeiculo
            {
                VeiculoId = veiculoId,
                TipoDocumento = tipoDocumento,
                NomeFicheiroOriginal = ficheiro.FileName,
                CaminhoFicheiro = $"/uploads/{nomeFicheiroUnico}", // Link público do anexo
                DataValidade = dataValidade
            };

            _context.DocumentosVeiculos.Add(novoDocumento);
            await _context.SaveChangesAsync();

            // ---- IDEIA 2: ALERTA DE NOTIFICAÇÃO AUTOMÁTICA ----
            // Se o documento anexado já estiver fora da validade, cria um alerta no sistema
            if (dataValidade.HasValue && dataValidade.Value < DateTime.Now)
            {
                var auditoriaService = HttpContext.RequestServices.GetService(typeof(GestaoDeFrotas.Services.AuditoriaService)) as GestaoDeFrotas.Services.AuditoriaService;
                if (auditoriaService != null)
                {
                    await auditoriaService.CriarNotificacaoAsync(
                        titulo: "Documento Caducado Detetado",
                        mensagem: $"Foi enviado um documento ({tipoDocumento}) já expirado para o veículo com o ID {veiculoId}.",
                        tipo: "Warning"
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

            // Apagar o ficheiro físico da pasta se ele existir
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

            // Devolve o ficheiro real (PDF ou Imagem) para o navegador abrir/fazer download
            return File(bytes, "application/octet-stream", doc.NomeFicheiroOriginal);
        }
    }
}