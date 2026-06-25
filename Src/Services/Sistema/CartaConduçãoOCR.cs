using System.Text.RegularExpressions;

namespace GestoreDeFrotas.Services.OCR
{
    public class CartaParserService
    {
        public DateTime? ExtrairValidade(string texto)
        {
            var match = Regex.Match(texto, @"\b(\d{2}/\d{2}/\d{4})\b");
            if (!match.Success) return null;

            if (DateTime.TryParse(match.Value, out var data))
                return data;

            return null;
        }

        public string? ExtrairNumero(string texto)
        {
            var match = Regex.Match(texto, @"\b\d{8,12}\b");
            return match.Success ? match.Value : null;
        }

        public string? ExtrairCategoria(string texto)
        {
            var match = Regex.Match(texto, @"\b(B|C|D|BE|CE|DE)\b");
            return match.Success ? match.Value : null;
        }
    }
}
