using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // 🔥 ENTIDADES PRINCIPAIS
        public DbSet<Vehicle> Veiculos { get; set; }
        public DbSet<LogSistema> LogsSistema { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }

        // 🔥 ENTIDADES QUE ESTAVAM EM FALTA
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<Abastecimento> Abastecimentos { get; set; }
        public DbSet<DocumentoVeiculo> DocumentosVeiculos { get; set; }
        public DbSet<MaintenanceRecord> RegistosManutencao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LOGS
            modelBuilder.Entity<LogSistema>(e =>
            {
                e.ToTable("LogsSistema");
                e.HasKey(l => l.Id);
                e.Property(l => l.Metodo).HasMaxLength(200);
                e.Property(l => l.MetodoHttp).HasMaxLength(20);
                e.Property(l => l.Rota).HasMaxLength(400);
                e.Property(l => l.Descricao).HasMaxLength(2000);
                e.Property(l => l.Utilizador).HasMaxLength(200);
            });

            // NOTIFICAÇÕES
            modelBuilder.Entity<Notificacao>(e =>
            {
                e.ToTable("Notificacoes");
                e.HasKey(n => n.Id);
                e.Property(n => n.Mensagem).HasMaxLength(500);
                e.Property(n => n.Tipo).HasMaxLength(50);
            });
        }
    }
}
