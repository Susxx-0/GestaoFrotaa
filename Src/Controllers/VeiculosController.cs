using FluentValidation;
using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly VeiculosService _veiculosService;
        private readonly AppDbContext _context;
        private readonly IValidator<Veiculo> _validator;

        public VeiculosController(VeiculosService veiculosService, AppDbContext context, IValidator<Veiculo> validator)
        {
            _veiculosService = veiculosService;
            _context = context;
            _validator = validator;
        }


        [HttpGet("exportar-excel")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> ExportarParaExcel()
        {
            var veiculos = await _context.Veiculos
                .Select(v => new
                {
                    ID = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Matrícula = v.Matricula,
                    Ano = v.Ano,
                    Cor = v.Cor,
                    Estado = v.Estado
                })
                .ToListAsync();

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(veiculos);
            memoryStream.Position = 0;

            var nomeFicheiro = $"Relatorio_Veiculos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeFicheiro);
        }

        // 📄 ENDPOINT: Exportar Relatório Geral em PDF
        [HttpGet("exportar-pdf")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> ExportarParaPdf()
        {
            var veiculos = await _context.Veiculos.ToListAsync();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Gestão de Frotas - Relatório Oficial").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingTop(20).Table(tabela =>
                    {
                        tabela.ColumnsDefinition(colunas =>
                        {
                            colunas.ConstantColumn(40);
                            colunas.RelativeColumn(2);
                            colunas.RelativeColumn(2);
                            colunas.RelativeColumn(2);
                            colunas.ConstantColumn(50);
                            colunas.RelativeColumn(2);
                        });

                        tabela.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("ID").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Marca").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Modelo").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Matrícula").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Ano").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5).Text("Estado").Bold().FontColor(Colors.White);
                        });

                        foreach (var v in veiculos)
                        {
                            tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Id.ToString());
                            tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Marca);
                            tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Modelo);
                            tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Matricula);
                            tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Ano.ToString());
                            tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Estado);
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            });

            var memoryStream = new MemoryStream();
            documento.GeneratePdf(memoryStream);
            memoryStream.Position = 0;

            var nomeFicheiro = $"Relatorio_Veiculos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(memoryStream, "application/pdf", nomeFicheiro);
        }

        // 📄 ENDPOINT: Exportar PDF de um Veículo Individual
        [HttpGet("{id}/exportar-pdf")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> ExportarParaPdfIndividual(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo == null)
            {
                return NotFound("Veículo não encontrado.");
            }

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("GESTÃO DE FROTAS").FontSize(10).Bold().FontColor(Colors.Grey.Medium);
                            col.Item().Text($"Ficha Técnica: {veiculo.Marca} {veiculo.Modelo}").FontSize(22).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingTop(30).Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Background(Colors.Grey.Lighten3).Padding(8).Text("Informações do Veículo").Bold().FontSize(12).FontColor(Colors.Blue.Darken3);

                        col.Item().Table(tabela =>
                        {
                            tabela.ColumnsDefinition(colunas =>
                            {
                                colunas.RelativeColumn(1);
                                colunas.RelativeColumn(1);
                            });

                            tabela.Cell().Padding(5).Text(t => { t.Span("ID do Sistema: ").Bold(); t.Span(veiculo.Id.ToString()); });
                            tabela.Cell().Padding(5).Text(t => { t.Span("Matrícula: ").Bold(); t.Span(veiculo.Matricula); });

                            tabela.Cell().Padding(5).Text(t => { t.Span("Marca: ").Bold(); t.Span(veiculo.Marca); });
                            tabela.Cell().Padding(5).Text(t => { t.Span("Modelo: ").Bold(); t.Span(veiculo.Modelo); });

                            tabela.Cell().Padding(5).Text(t => { t.Span("Ano de Fabrico: ").Bold(); t.Span(veiculo.Ano.ToString()); });
                            tabela.Cell().Padding(5).Text(t => { t.Span("Cor: ").Bold(); t.Span(veiculo.Cor ?? "Não Definida"); });
                        });

                        col.Item().PaddingTop(15);

                        col.Item().Background(Colors.Grey.Lighten3).Padding(8).Text("Status Atual").Bold().FontSize(12).FontColor(Colors.Blue.Darken3);

                        col.Item().Text(t =>
                        {
                            t.Span("Estado Operacional: ").Bold();
                            t.Span(veiculo.Estado).Bold().FontColor(veiculo.Estado == "Disponível" ? Colors.Green.Darken2 : Colors.Red.Darken2);
                        });

                        col.Item().PaddingTop(50).BorderTop(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5)
                            .Text("Este documento reflete o estado atual do registo do veículo na base de dados centralizada do sistema Gestão de Frotas.").FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Ficha Técnica Individual - Confidencial");
                    });
                });
            });

            var memoryStream = new MemoryStream();
            documento.GeneratePdf(memoryStream);
            memoryStream.Position = 0;

            var nomeFicheiro = $"Ficha_Veiculo_{veiculo.Matricula}_{DateTime.Now:yyyyMMdd}.pdf";

            return File(memoryStream, "application/pdf", nomeFicheiro);
        }
        [HttpGet("advanced-search")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> AdvancedSearch(
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
            query = ApplySorting(query, sort);

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

        private IQueryable<Veiculo> ApplySorting(IQueryable<Veiculo> query, string? sort)
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

        // ➕ ENDPOINT: Criar Veículo
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create([FromBody] Veiculo veiculo)
        {
            var validationResult = await _validator.ValidateAsync(veiculo);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            await _veiculosService.AdicionarAsync(veiculo);
            return Ok(veiculo);
        }

        // ❌ ENDPOINT: Eliminar Veículo
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _veiculosService.EliminarAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}