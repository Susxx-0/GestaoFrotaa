using System;
using System.ComponentModel.DataAnnotations;
namespace GestoreDeFrotas.Models
{
    public class Viagem
    {
        public int Id { get; set; }

        [Required]
        public int VeiculoId { get; set; }

        [Required]
        public string CondutorPrincipalId { get; set; } = string.Empty;

        public string? CondutorSecundarioId { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public string? ObservacoesEntrega { get; set; }

        public bool EstaAtiva { get; set; } = true;

        public DateTime DataLimitePrevista { get; set; }

        public bool PediuProrrogacao { get; set; } = false;
    }
}