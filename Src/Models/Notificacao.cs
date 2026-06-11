using System;
using System.ComponentModel.DataAnnotations;

namespace GestoreDeFrotas.Models
{
    public class Notificacao
    {
        public int Id { get; set; }

        [Required]
        public string Mensagem { get; set; } = string.Empty; // Descrição do aviso (ex: "Barulho detetado na carrinha X")

        [Required]
        public string DestinatarioId { get; set; } = string.Empty; // Pode ser o ID único de um utilizador ou o nome de um Perfil/Role (Admin, Tecnico)

        [Required]
        public string Grau { get; set; } = "Info"; // Níveis de gravidade: "Info", "Aviso", "Crítico"

        public int? VeiculoId { get; set; } // Opcional: Associar a notificação a um carro específico

        public DateTime DataCriacao { get; set; } = DateTime.Now; // Registo de quando o alerta foi gerado

        public bool Lida { get; set; } = false; // Controlo de leitura no frontend
    }
}