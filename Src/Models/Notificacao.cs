using System;

namespace GestoreDeFrotas.Models
{
    public class Notificacao
    {
        public int Id { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Grau { get; set; } = "Info";
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Lida { get; set; } = false;

        public int? VeiculoId { get; set; }
    }
}
