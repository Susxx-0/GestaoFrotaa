using System;

namespace GestaoDeFrotas.Models
{
    public class Notificacao
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Mensagem { get; set; } = string.Empty;

        public string Tipo { get; set; } = "Info";

        public DateTime Data { get; set; } = DateTime.Now;

        public bool Lida { get; set; } = false;
    }
}
