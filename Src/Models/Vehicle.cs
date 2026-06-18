using System;
using System.ComponentModel.DataAnnotations;

namespace GestoreDeFrotas.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        [Required]
        public string Marca { get; set; } = null!;

        [Required]
        public string Modelo { get; set; } = null!;

        [Required]
        public string Matricula { get; set; } = null!;

        public int Ano { get; set; }

        public string CategoriaUsuario { get; set; } = null!;

        public string Estado { get; set; } = "Disponível";

        public bool EstaAtivo { get; set; } = true;

        // NOVO: Cor
        public string? Cor { get; set; }

        // KM
        public int KmAtual { get; set; }

        // Manutenção
        public int? UltimaManutencaoKm { get; set; }
        public DateTime? UltimaManutencaoData { get; set; }

        public int? ProximaManutencaoKm { get; set; }
        public DateTime? ProximaManutencaoData { get; set; }

        // NOVO: Próxima IPO (inspeção)
        public DateTime? DataProximaIpo { get; set; }

        // Datas importantes
        public DateTime? DataInspecao { get; set; }
        public DateTime? DataSeguro { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
