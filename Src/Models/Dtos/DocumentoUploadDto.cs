using Microsoft.AspNetCore.Http;
using System;

namespace GestoreDeFrotas.Models
{
    public class DocumentoUploadDto
    {
        public int VeiculoId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public IFormFile? Ficheiro { get; set; }
        public DateTime? DataValidade { get; set; }
    }
}
