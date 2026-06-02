using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoDeFrotas.Models
{
    public class LogSistema
    {
        public int Id { get; set; }

        public string? Utilizador { get; set; }

        [Required]
        public string MetodoHttp { get; set; } = string.Empty;

        [Required]
        public string Rota { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
    }
}
