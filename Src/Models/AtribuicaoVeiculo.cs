using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestoreDeFrotas.Models
{
    [Table("AtribuicoesVeiculo")]
    public class AtribuicaoVeiculo
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
