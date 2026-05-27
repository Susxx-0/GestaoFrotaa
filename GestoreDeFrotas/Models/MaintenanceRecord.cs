using System;

namespace GestaoDeFrotas.Models
{
    public class RegistoManutencao
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; } // Ligação ao ID do veículo
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Custo { get; set; }
        public string RealizadoPor { get; set; } = string.Empty; // Nome do utilizador do Token
    }
}