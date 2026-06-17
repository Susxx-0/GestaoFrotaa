using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<Abastecimento> Abastecimentos { get; set; }
        public DbSet<RegistoManutencao> Manutencoes { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<LogSistema> LogsSistema { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RegistoManutencao>()
                .Property(m => m.Custo)
                .HasColumnType("decimal(18,2)");
        }
    }
}