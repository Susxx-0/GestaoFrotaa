using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

namespace GestoreDeFrotas.Services.Relatorios
{
    public class PDFService
    {
        public byte[] GerarRelatorioVeiculo(object dados)
        {
            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Título
            doc.Add(new Paragraph("Relatório do Veículo")
                .SetFont(bold)
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER));

            // Dados gerais
            doc.Add(new Paragraph("\nDados Gerais").SetFont(bold).SetFontSize(14));

            foreach (var prop in dados.GetType().GetProperties())
            {
                if (prop.Name.Contains("Historico"))
                    continue;

                doc.Add(new Paragraph($"{prop.Name}: {prop.GetValue(dados)}"));
            }

            // Tabela de viagens
            doc.Add(new Paragraph("\nHistórico de Viagens").SetFont(bold).SetFontSize(14));

            var viagens = (IEnumerable<object>)dados.GetType()
                .GetProperty("HistoricoDeViagens")!
                .GetValue(dados)!;

            var tabelaViagens = new Table(1).UseAllAvailableWidth();
            tabelaViagens.AddHeaderCell(new Cell().Add(new Paragraph("Viagem").SetFont(bold)));

            foreach (var v in viagens)
                tabelaViagens.AddCell(new Cell().Add(new Paragraph(v.ToString())));

            doc.Add(tabelaViagens);

            // Tabela de abastecimentos
            doc.Add(new Paragraph("\nHistórico de Abastecimentos").SetFont(bold).SetFontSize(14));

            var abastecimentos = (IEnumerable<object>)dados.GetType()
                .GetProperty("HistoricoDeAbastecimentos")!
                .GetValue(dados)!;

            var tabelaAbast = new Table(1).UseAllAvailableWidth();
            tabelaAbast.AddHeaderCell(new Cell().Add(new Paragraph("Abastecimento").SetFont(bold)));

            foreach (var a in abastecimentos)
                tabelaAbast.AddCell(new Cell().Add(new Paragraph(a.ToString())));

            doc.Add(tabelaAbast);

            doc.Close();
            return ms.ToArray();
        }
    }
}