using System;

namespace GestoreDeFrotas.Models
{
    public class RegistoManutencao
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Custo { get; set; }
        public string RealizadoPor { get; set; } = string.Empty;
        public int Km { get; set; }
    }
}