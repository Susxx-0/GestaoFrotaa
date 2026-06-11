using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestoreDeFrotas.Models
{
    public class Abastecimento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do veículo é obrigatório.")]
        public int VeiculoId { get; set; }

        [ForeignKey("VeiculoId")]
        public Veiculo? Veiculo { get; set; }

        [Required(ErrorMessage = "Os litros são obrigatórios.")]
        [Range(1, 200, ErrorMessage = "Os litros devem ser entre 1 e 200.")]
        public double Litros { get; set; }

        [Required(ErrorMessage = "O preço por litro é obrigatório.")]
        [Range(0.1, 10, ErrorMessage = "O preço por litro deve ser entre 0.1 e 10.")]
        public double PrecoPorLitro { get; set; }

        public double CustoTotal { get; set; }

        [Required(ErrorMessage = "Os KM atuais são obrigatórios.")]
        [Range(0, 1000000, ErrorMessage = "Os KM devem ser entre 0 e 1.000.000.")]
        public int KmAtual { get; set; }

        public double? ConsumoMedio { get; set; } // L/100km
        public double? KmPorLitro { get; set; }

        [Required(ErrorMessage = "O tipo de combustível é obrigatório.")]
        public string Combustivel { get; set; } = "Gasolina";

        public string Posto { get; set; } = "Desconhecido";

        public DateTime Data { get; set; } = DateTime.Now;

        public string? FaturaPath { get; set; }
    }
}