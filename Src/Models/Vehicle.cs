using System;

namespace GestoreDeFrotas.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public int Ano { get; set; }
        /// Estados possíveis: "Disponível", "Em uso", "Em Manutenção"
        public string Estado { get; set; } = "Disponível";
        public string CategoriaUsuario { get; set; } = string.Empty; // Empresa, Outros
        public bool EstaAtivo { get; set; } = true;
        public int KmAtual { get; set; }
        public int UltimaManutencaoKm { get; set; }
        public DateTime? UltimaManutencaoData { get; set; }
        public int ProximaManutencaoKm => UltimaManutencaoKm + 15000; // Regra de negócio: Manutenção a cada 15.000 KM
        public DateTime ProximaManutencaoData => UltimaManutencaoData?.AddMonths(12) ?? DateTime.Now.AddMonths(12); // Ou a cada 12 meses
    }
}