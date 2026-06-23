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

        // ============================
        //           DBSETS
        // ============================

        public DbSet<Abastecimento> Abastecimentos { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<RegistoManutencao> RegistosManutencao { get; set; }
        public DbSet<DocumentoVeiculo> DocumentosVeiculos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<HistoricoVeiculo> HistoricoVeiculos { get; set; }
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<LogSistema> LogsSistema { get; set; }
     
        public DbSet<User> Users { get; set; }
        public DbSet<AtribuicaoVeiculo> AtribuicoesVeiculo { get; set; }

        // ============================
        //        CONFIGURAÇÕES
        // ============================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PRECISÃO DECIMAL
            modelBuilder.Entity<Abastecimento>()
        .Property(a => a.CustoTotal)
        .HasPrecision(18, 2);

            modelBuilder.Entity<Abastecimento>()
                .Property(a => a.PrecoPorLitro)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Abastecimento>()
                .Property(a => a.Litros)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RegistoManutencao>()
                .Property(m => m.Custo)
                .HasPrecision(18, 2);

            // RELAÇÃO: Veículo -> Abastecimentos
            modelBuilder.Entity<Abastecimento>()
                .HasOne(a => a.Veiculo)
                .WithMany()
                .HasForeignKey(a => a.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);

            // RELAÇÃO: Veículo -> Documentos
            modelBuilder.Entity<DocumentoVeiculo>()
                .HasOne<Veiculo>()
                .WithMany()
                .HasForeignKey(d => d.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);

            // RELAÇÃO: Veículo -> Manutenção
            modelBuilder.Entity<RegistoManutencao>()
                .HasOne(r => r.Veiculo)
                .WithMany()
                .HasForeignKey(r => r.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);

            // RELAÇÃO: Veículo -> Viagens
            modelBuilder.Entity<Viagem>()
                .HasOne(v => v.Veiculo)
                .WithMany()
                .HasForeignKey(v => v.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // RELAÇÃO: Notificações -> Veículo 
            modelBuilder.Entity<Notificacao>()
                .HasOne<Veiculo>()
                .WithMany()
                .HasForeignKey(n => n.VeiculoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
