using System;

namespace GestoreDeFrotas.Models
{
    public class Notificacao
    {
        public int Id { get; set; }
        public int? VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string DestinatarioId { get; set; } = string.Empty;
        public string Grau { get; set; } = "Aviso"; // Aviso, TecnicoResponsavel, Admin, Error
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Lida { get; set; } = false;
    }
}