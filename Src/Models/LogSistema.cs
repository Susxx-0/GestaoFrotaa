using System;

namespace GestoreDeFrotas.Models
{
    public class LogSistema
    {
        public int Id { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public DateTime DataRegisto { get; set; } = DateTime.Now;

        public string Metodo { get; set; } = string.Empty;

        public string MetodoHttp { get; set; } = string.Empty;

        public string Rota { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string Utilizador { get; set; } = string.Empty;
    }
}
