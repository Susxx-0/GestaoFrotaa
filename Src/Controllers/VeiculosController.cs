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
using System.Security.Claims;
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

        [HttpGet("{id}/exportar-pdf")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador,Outros")]
        public async Task<IActionResult> ExportarParaPdfIndividual(int id)
        {
            if (User.IsInRole("Outros"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var possuiViagemNoVeiculo = await _context.Viagens
                    .AnyAsync(v => v.VeiculoId == id && (v.CondutorPrincipalId == userId || v.CondutorSecundarioId == userId));

                if (!possuiViagemNoVeiculo)
                {
                    return Forbid("Acesso negado. Apenas pode exportar dados de um veiculo que lhe esteja associado.");
                }
            }

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

                            tabela.Cell().Padding(5).Text($"ID do Sistema: {veiculo.Id}");
                            tabela.Cell().Padding(5).Text($"Matrícula: {veiculo.Matricula}");

                            tabela.Cell().Padding(5).Text($"Marca: {veiculo.Marca}");
                            tabela.Cell().Padding(5).Text($"Modelo: {veiculo.Modelo}");

                            tabela.Cell().Padding(5).Text($"Ano de Fabrico: {veiculo.Ano}");
                            tabela.Cell().Padding(5).Text($"Cor: {(veiculo.Cor ?? "Não Definida")}");
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

        [HttpGet("{id}/historico-excel")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Outros")]
        public async Task<IActionResult> ExportarHistoricoExcel(int id)
        {
            if (User.IsInRole("Outros"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var possuiViagem = await _context.Viagens.AnyAsync(v => v.VeiculoId == id && (v.CondutorPrincipalId == userId || v.CondutorSecundarioId == userId));
                if (!possuiViagem) return Forbid("Acesso negado. Apenas pode consultar dados de veículos associados à sua conta.");
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) return NotFound("Veículo não encontrado.");

            var viagens = await _context.Viagens
                .Where(v => v.VeiculoId == id)
                .OrderByDescending(v => v.DataInicio)
                .Select(v => new
                {
                    Registo = "Viagem / Condução",
                    Data_Inicial = v.DataInicio.ToString("dd/MM/yyyy HH:mm"),
                    Data_Final = v.DataFim.HasValue ? v.DataFim.Value.ToString("dd/MM/yyyy HH:mm") : "Em Curso",
                    Duracao_Horas = v.DataFim.HasValue ? (v.DataFim.Value - v.DataInicio).TotalHours.ToString("F1") : "N/A",
                    Condutor = v.CondutorPrincipalId,
                    Tecnico_Entrega = v.TecnicoLevantamentoId ?? "N/A",
                    Tecnico_Rececao = v.TecnicoRecebimentoId ?? "N/A",
                    KM_Iniciais = v.KmIniciais.ToString(),
                    KM_Finais = v.KmFinais.HasValue ? v.KmFinais.Value.ToString() : "Em Uso",
                    Detalhes = v.ObservacoesEntrega ?? ""
                })
                .ToListAsync();

            var abastecimentos = await _context.Abastecimentos
                .Where(a => a.VeiculoId == id)
                .OrderByDescending(a => a.Data)
                .Select(a => new
                {
                    Registo = "Abastecimento",
                    Data_Inicial = a.Data.ToString("dd/MM/yyyy HH:mm"),
                    Data_Final = a.Data.ToString("dd/MM/yyyy HH:mm"),
                    Duracao_Horas = "N/A",
                    Condutor = "N/A",
                    Tecnico_Entrega = "N/A",
                    Tecnico_Rececao = "N/A",
                    KM_Iniciais = a.KmAtual.ToString(),
                    KM_Finais = a.KmAtual.ToString(),
                    Detalhes = $"Posto: {a.Posto} | {a.Litros}L {a.Combustivel} (Total: {a.CustoTotal}€)"
                })
                .ToListAsync();

            var historicoCompleto = viagens.Cast<object>().Concat(abastecimentos.Cast<object>()).ToList();

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(historicoCompleto);
            memoryStream.Position = 0;

            var nomeFicheiro = $"Historico_Veiculo_{veiculo.Matricula}_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeFicheiro);
        }

        [HttpGet("{id}/historico-pdf")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Outros")]
        public async Task<IActionResult> ExportarHistoricoPdf(int id)
        {
            if (User.IsInRole("Outros"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var possuiViagem = await _context.Viagens.AnyAsync(v => v.VeiculoId == id && (v.CondutorPrincipalId == userId || v.CondutorSecundarioId == userId));
                if (!possuiViagem) return Forbid("Acesso negado.");
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) return NotFound("Veículo não encontrado.");

            var viagens = await _context.Viagens.Where(v => v.VeiculoId == id).OrderByDescending(v => v.DataInicio).ToListAsync();
            var abastecimentos = await _context.Abastecimentos.Where(a => a.VeiculoId == id).OrderByDescending(a => a.Data).ToListAsync();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4.Landscape());
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text($"Histórico Unificado do Veículo: {veiculo.Marca} {veiculo.Modelo} [{veiculo.Matricula}]").FontSize(16).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);

                        // Correção da Linha Horizontal de forma nativa e válida no QuestPDF
                        col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingBottom(8);
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Item().Text("1. Registos de Condução e Responsáveis Técnicos").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
                        col.Item().PaddingTop(5).Table(tabela =>
                        {
                            tabela.ColumnsDefinition(colunas =>
                            {
                                colunas.RelativeColumn(2);
                                colunas.RelativeColumn(2.5f);
                                colunas.RelativeColumn(2.5f);
                                colunas.ConstantColumn(50);
                                colunas.RelativeColumn(1.5f);
                                colunas.RelativeColumn(1.5f);
                                colunas.ConstantColumn(50);
                                colunas.ConstantColumn(50);
                            });

                            tabela.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("Condutor").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("Início").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("Fim").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("Tempo").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("Téc. Leva").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("Téc. Rec").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("KM Ini").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4).Text("KM Fim").Bold().FontColor(Colors.White);
                            });

                            foreach (var v in viagens)
                            {
                                var duracao = v.DataFim.HasValue ? $"{(v.DataFim.Value - v.DataInicio).TotalHours:F1}h" : "Ativa";
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.CondutorPrincipalId);
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.DataInicio.ToString("dd/MM/yyyy HH:mm"));
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? "Em curso");
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(duracao);
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.TecnicoLevantamentoId ?? "N/A");
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.TecnicoRecebimentoId ?? "N/A");
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.KmIniciais.ToString());
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.KmFinais?.ToString() ?? "-");
                            }
                        });

                        col.Item().PaddingTop(20);

                        col.Item().Text("2. Histórico de Abastecimentos e Quilometragem do Painel").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
                        col.Item().PaddingTop(5).Table(tabela =>
                        {
                            tabela.ColumnsDefinition(colunas =>
                            {
                                colunas.RelativeColumn(2.5f);
                                colunas.RelativeColumn(2);
                                colunas.RelativeColumn(1.5f);
                                colunas.RelativeColumn(1.5f);
                                colunas.RelativeColumn(1.5f);
                                colunas.RelativeColumn(1.5f);
                                colunas.RelativeColumn(2);
                            });

                            tabela.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("Data / Hora").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("Posto").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("Tipo").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("Litros").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("Preço/L").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("Total").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken2).Padding(4).Text("KM Atual").Bold().FontColor(Colors.White);
                            });

                            foreach (var a in abastecimentos)
                            {
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(a.Data.ToString("dd/MM/yyyy HH:mm"));
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(a.Posto);
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(a.Combustivel);
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{a.Litros} L");
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{a.PrecoPorLitro:F2}€");
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{a.CustoTotal:F2}€");
                                tabela.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{a.KmAtual} km");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(t => t.Span("Ficha de Histórico Centralizada - Uso Interno"));
                });
            });

            var memoryStream = new MemoryStream();
            documento.GeneratePdf(memoryStream);
            memoryStream.Position = 0;

            var nomeFicheiro = $"Historico_Veiculo_{veiculo.Matricula}_{DateTime.Now:yyyyMMdd}.pdf";
            return File(memoryStream, "application/pdf", nomeFicheiro);
        }

        [HttpGet("advanced-search")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador,Outros")]
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

            if (User.IsInRole("Outros"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var veiculosAssociadosIds = await _context.Viagens
                    .Where(v => v.CondutorPrincipalId == userId || v.CondutorSecundarioId == userId)
                    .Select(v => v.VeiculoId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(v => veiculosAssociadosIds.Contains(v.Id));
            }

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

            // Aplicar a ordenação antes da paginação
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

        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create([FromBody] Veiculo veiculo)
        {
            veiculo.Id = 0;

            var validationResult = await _validator.ValidateAsync(veiculo);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            await _veiculosService.AdicionarAsync(veiculo);
            return Ok(veiculo);
        }

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