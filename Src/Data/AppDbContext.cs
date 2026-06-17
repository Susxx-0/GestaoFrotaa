using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Abastecimento> Abastecimentos { get; set; }
        public DbSet<LogSistema> LogsSistema { get; set; }

        // Mapeos requeridos por tus controladores de documentos y mantenimiento
        public DbSet<DocumentoVeiculo> DocumentosVeiculos { get; set; }
        public DbSet<RegistoManutencao> RegistosManutencao { get; set; }

        // Propiedad de compatibilidad por si algún servicio busca la tabla en inglés/plural
        public DbSet<RegistoManutencao> Manutencoes => RegistosManutencao;
    }
}