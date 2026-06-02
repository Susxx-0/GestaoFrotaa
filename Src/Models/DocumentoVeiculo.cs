using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoDeFrotas.Models
{
    public class DocumentoVeiculo
    {
        public int Id { get; set; }

        [Required]
        public int VeiculoId { get; set; }

        [Required]
        public string TipoDocumento { get; set; } = string.Empty; // Seguro, IPO, Livrete, etc.

        [Required]
        public string NomeFicheiroOriginal { get; set; } = string.Empty;

        [Required]
        public string CaminhoFicheiro { get; set; } = string.Empty; // Onde fica guardado no PC/Servidor

        public DateTime? DataValidade { get; set; } // Data em que o documento caduca

        public DateTime DataUpload { get; set; } = DateTime.Now;
    }
}   