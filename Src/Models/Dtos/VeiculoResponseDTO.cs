namespace GestoreDeFrotas.Dtos.Veiculos
{
    public class VeiculoResponseDTO
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Matricula { get; set; }
        public int Ano { get; set; }
        public string CategoriaUsuario { get; set; }
        public string Estado { get; set; }
        public string Cor { get; set; }
        public int KmAtual { get; set; }
        public bool EstaAtivo { get; set; }
        public DateTime? DataProximaIpo { get; set; }
        public DateTime? DataSeguro { get; set; }
    }
}
