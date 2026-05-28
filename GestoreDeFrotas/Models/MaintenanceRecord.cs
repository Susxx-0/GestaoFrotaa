using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoDeFrotas.Models
{
    public class RegistoManutencao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do veículo é obrigatório.")]
        public int VeiculoId { get; set; }

        [Required(ErrorMessage = "A data da manutenção é obrigatória.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(250, ErrorMessage = "A descrição não pode levar mais de 250 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Range(0.01, 100000.00, ErrorMessage = "O custo da manutenção deve ser maior do que zero.")]
        public decimal Custo { get; set; }

        public string RealizadoPor { get; set; } = "Técnico";
    }
}