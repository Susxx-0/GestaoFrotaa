using GestoreDeFrotas.Services.Relatorios;
using GestoreDeFrotas.Services.Viagens;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly HistoricoVeiculoService _historico;
        private readonly PDFService _pdf;
        private readonly ExcelService _excel;

        public RelatoriosController(
            HistoricoVeiculoService historico,
            PDFService pdf,
            ExcelService excel)
        {
            _historico = historico;
            _pdf = pdf;
            _excel = excel;
        }

        [HttpGet("veiculo/{id}/pdf")]
        public async Task<IActionResult> Pdf(int id)
        {
            var dados = await _historico.ObterHistoricoCompletoAsync(id);
            var bytes = _pdf.GerarRelatorioVeiculo(dados);
            return File(bytes, "application/pdf", $"Relatorio_{id}.pdf");
        }

        [HttpGet("veiculo/{id}/excel")]
        public async Task<IActionResult> Excel(int id)
        {
            var dados = await _historico.ObterHistoricoCompletoAsync(id);
            var bytes = _excel.GerarRelatorioVeiculo(dados);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Relatorio_{id}.xlsx");
        }
    }
}