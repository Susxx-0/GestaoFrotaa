using System.ComponentModel.DataAnnotations;

namespace GestaoDeFrotas.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [StringLength(50, ErrorMessage = "A marca não pode ter mais de 50 caracteres.")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "O modelo é obrigatório.")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A matrícula é obrigatória.")]
        [RegularExpression(@"^[A-Z0-9]{2}-[A-Z0-9]{2}-[A-Z0-9]{2}$", ErrorMessage = "A matrícula deve seguir o formato XX-XX-XX.")]
        public string Matricula { get; set; } = string.Empty;

        public string Cor { get; set; } = "Não Especificada";

        [Range(1970, 2027, ErrorMessage = "O ano do veículo deve ser entre 1970 e 2027.")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "O estado do veículo é obrigatório.")]
        public string Estado { get; set; } = "Disponível"; // Disponível, Alugado, Em Manutenção
    }
}