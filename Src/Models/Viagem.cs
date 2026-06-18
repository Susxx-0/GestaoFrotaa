using System;

namespace GestoreDeFrotas.Models
{
    public class Viagem
    {
        public int Id { get; set; }

        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        public string CondutorPrincipalId { get; set; } = string.Empty;
        public string? CondutorSecundarioId { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public int KmIniciais { get; set; }
        public int? KmFinais { get; set; }

        public DateTime DataLimitePrevista { get; set; }

        public bool EstaAtiva { get; set; } = true;
        public bool PediuProrrogacao { get; set; } = false;

        public string? ObservacoesEntrega { get; set; }
    }
}
