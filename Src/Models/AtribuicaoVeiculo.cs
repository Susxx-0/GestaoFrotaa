using System;
using GestoreDeFrotas.Models;

namespace GestoreDeFrotas.Models
{
    public class AtribuicaoVeiculo
    {
        public int Id { get; set; }

        public int VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime DataInicio { get; set; } = DateTime.Now;
        public DateTime? DataFim { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
