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
        public string Cor { get; set; } = string.Empty; // Resuelve el error en VeiculosPesquisaController
        public string Estado { get; set; } = "Disponível"; // Disponível, Em uso, Em Manutenção
        public string CategoriaUsuario { get; set; } = string.Empty;
        public bool EstaAtivo { get; set; } = true;
        public int KmAtual { get; set; }

        // Control de Mantenimiento e Inspección (IPO)
        public int UltimaManutencaoKm { get; set; }
        public DateTime? UltimaManutencaoData { get; set; }

        // Definido como int? para solucionar los errores de '.HasValue' y '.Value'
        public int? IntervaloManutencaoKm { get; set; } = 15000;
        public DateTime? DataProximaIpo { get; set; } // Resuelve el error en DashboardService

        // Propiedades calculadas limpias
        public int ProximaManutencaoKm => UltimaManutencaoKm + (IntervaloManutencaoKm ?? 15000);
        public DateTime ProximaManutencaoData => UltimaManutencaoData?.AddMonths(12) ?? DateTime.Now.AddMonths(12);
    }
}