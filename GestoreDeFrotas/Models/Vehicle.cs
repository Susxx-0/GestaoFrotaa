namespace GestaoDeFrotas.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Estado { get; set; } = "Disponível"; // Disponível, Alugado, Em Manutenção
    }
}