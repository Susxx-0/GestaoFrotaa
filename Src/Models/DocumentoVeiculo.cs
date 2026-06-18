using System;
using System.ComponentModel.DataAnnotations;

namespace GestoreDeFrotas.Models
{
    public class DocumentoVeiculo
    {
        public int Id { get; set; }

        [Required]
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        [Required]
        public string TipoDocumento { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        [Required]
        public string NomeFicheiroOriginal { get; set; } = string.Empty;

        [Required]
        public string CaminhoFicheiro { get; set; } = string.Empty;
      
        public DateTime? DataValidade { get; set; }

        public DateTime DataUpload { get; set; } = DateTime.Now;
    }
}
