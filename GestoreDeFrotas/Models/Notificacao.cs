using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoDeFrotas.Models
{
    public class Notificacao
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Mensagem { get; set; } = string.Empty;

        public string Tipo { get; set; } = "Info"; // Info, Warning, Error

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public bool Lida { get; set; } = false;
    }
}