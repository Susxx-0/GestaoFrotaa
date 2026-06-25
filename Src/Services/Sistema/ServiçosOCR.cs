using Tesseract;

namespace GestoreDeFrotas.Services.OCR
{
    public class OcrService
    {
        private readonly string _tessdataPath = "./tessdata";

        public string LerTexto(string caminhoImagem)
        {
            using var engine = new TesseractEngine(_tessdataPath, "por", EngineMode.Default);
            using var img = Pix.LoadFromFile(caminhoImagem);
            using var page = engine.Process(img);

            return page.GetText();
        }
    }
}
