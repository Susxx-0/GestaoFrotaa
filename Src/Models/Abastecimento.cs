using System;
using System.ComponentModel.DataAnnotations;

namespace GestoreDeFrotas.Models
{
    public class Abastecimento
    {
        public int Id { get; set; }

        [Required]
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        [Required]
        public decimal Litros { get; set; }

        [Required]
        public decimal PrecoPorLitro { get; set; }

        public decimal CustoTotal { get; set; }

        public int KmAtual { get; set; }

        public decimal ConsumoMedio { get; set; }

        public string Combustivel { get; set; } = string.Empty;
        public string Posto { get; set; } = string.Empty;

        public DateTime Data { get; set; } = DateTime.Now;
    }
}
