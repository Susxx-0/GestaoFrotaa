using System;

namespace GestoreDeFrotas.Models
{
    public class Abastecimento
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        public double Litros { get; set; }
        public double PrecoPorLitro { get; set; }
        public double CustoTotal { get; set; }

        public int KmAtual { get; set; }
        public double? KmPorLitro { get; set; }
        public double? ConsumoMedio { get; set; }

        public string Combustivel { get; set; } = string.Empty;
        public string Posto { get; set; } = string.Empty;

        public DateTime Data { get; set; }
    }
}
