using System;

namespace GestoreDeFrotas.Models
{
    public class LogSistema
    {
        public int Id { get; set; }
        public string Utilizador { get; set; } = string.Empty;
        public string MetodoHttp { get; set; } = string.Empty;
        public string Rota { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime DataRegisto { get; set; } = DateTime.Now;
    }
}