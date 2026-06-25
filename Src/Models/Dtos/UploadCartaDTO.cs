using Microsoft.AspNetCore.Http;

namespace GestoreDeFrotas.DTOs
{
    public class UploadCartaDTO
    {
        public int UserId { get; set; }
        public IFormFile Ficheiro { get; set; }
    }
}
