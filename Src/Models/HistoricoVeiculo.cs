namespace GestoreDeFrotas.Models
{
    public class HistoricoVeiculo
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime Data { get; set; }

        public Veiculo? Veiculo { get; set; }
    }
}
