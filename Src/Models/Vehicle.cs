using System;
using System.ComponentModel.DataAnnotations;

namespace GestoreDeFrotas.Models

{

    public class Veiculo
    {

 
        public string Cor { get; set; } = "Não Especificada";
        public DateTime? UltimaManutencaoData { get; set; }

        public int Id { get; set; }

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public string Matricula { get; set; } = string.Empty;

        public int Ano { get; set; }

        public string Estado { get; set; } = "Disponível";

        public int KmAtual { get; set; } = 0;

        public int? UltimaManutencaoKm { get; set; }

        public int? IntervaloManutencaoKm { get; set; } = 15000;

        public bool EstaAtivo { get; set; } = true;

        public string? TecnicoResponsavelId { get; set; }

        public string? CondutorHabitualId { get; set; }

        public string CategoriaUsuario { get; set; } = "Empresa";

        public DateTime? DataProximaIpo { get; set; }
    }
}