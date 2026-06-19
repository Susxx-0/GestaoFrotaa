using System;
using System.ComponentModel.DataAnnotations;

namespace GestoreDeFrotas.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public string Matricula { get; set; } = string.Empty;

        public int Ano { get; set; }

        public string CategoriaUsuario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;

        public int KmAtual { get; set; }

        
        public int UltimaManutencaoKm { get; set; }
        public DateTime? UltimaManutencaoData { get; set; }
        public int ProximaManutencaoKm { get; set; }
        public DateTime? ProximaManutencaoData { get; set; }

       
        public DateTime? DataProximaIpo { get; set; }
        public DateTime? DataInspecao { get; set; }
        public DateTime? DataSeguro { get; set; }

        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
