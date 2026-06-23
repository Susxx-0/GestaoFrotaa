using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace GestoreDeFrotas.Services.Relatorios
{
    public class ExcelService
    {
        public byte[] GerarRelatorioVeiculo(object dados)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Relatório");

            int row = 1;

            // Dados gerais
            foreach (var prop in dados.GetType().GetProperties())
            {
                if (prop.Name.Contains("Historico"))
                    continue;

                ws.Cells[row, 1].Value = prop.Name;
                ws.Cells[row, 2].Value = prop.GetValue(dados)?.ToString();
                row++;
            }

            row += 2;

            // Tabela de viagens
            ws.Cells[row, 1].Value = "Histórico de Viagens";
            ws.Cells[row, 1].Style.Font.Bold = true;
            row++;

            var viagens = (IEnumerable<object>)dados.GetType()
                .GetProperty("HistoricoDeViagens")!
                .GetValue(dados)!;

            int viagensStart = row;

            foreach (var v in viagens)
            {
                ws.Cells[row, 1].Value = v.ToString();
                row++;
            }

            int viagensEnd = row - 1;

            // Gráfico
            var chart = ws.Drawings.AddChart("graficoViagens", eChartType.Line);
            chart.Title.Text = "Gráfico de Viagens";
            chart.SetPosition(1, 0, 3, 0);
            chart.SetSize(600, 400);

            var serie = chart.Series.Add(
                ws.Cells[viagensStart, 1, viagensEnd, 1],
                ws.Cells[viagensStart, 1, viagensEnd, 1]
            );

            serie.Header = "Viagens";

            return package.GetAsByteArray();
        }
    }
}