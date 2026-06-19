namespace GestoreDeFrotas.Models.Dtos.Abastecimentos
{
    public class AbastecimentoUpdateDTO
    {
        public decimal Litros { get; set; }
        public decimal PrecoPorLitro { get; set; }
        public int KmAtual { get; set; }
        public string Combustivel { get; set; } = string.Empty;
        public string Posto { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }
}
