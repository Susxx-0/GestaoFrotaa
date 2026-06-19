namespace GestoreDeFrotas.Models
{
    public class Abastecimento
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        public decimal Litros { get; set; }
        public decimal PrecoPorLitro { get; set; }
        public decimal CustoTotal { get; set; }

        public string Combustivel { get; set; } = string.Empty;
        public string Posto { get; set; } = string.Empty;

        public double KmAtual { get; set; }
        public double ConsumoMedio { get; set; }

        public DateTime Data { get; set; }
    }
}
