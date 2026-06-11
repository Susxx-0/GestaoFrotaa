using Microsoft.EntityFrameworkCore;
using GestoreDeFrotas.Models;
using System;
using System.Linq;

namespace GestoreDeFrotas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public string Cor { get; set; } = string.Empty;
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<RegistoManutencao> RegistosManutencao { get; set; }
        public DbSet<DocumentoVeiculo> DocumentosVeiculos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<LogSistema> LogsSistema { get; set; }
        public DbSet<Viagem> Viagens { get; set; }
        public DbSet<Abastecimento> Abastecimentos { get; set; }

        public static void SeedData(AppDbContext context)
        {
            if (!context.Veiculos.Any())
            {
                var marcasModelos = new[] { "Renault Clio", "Peugeot 208", "BMW Serie 3", "Tesla Model 3", "Mercedes Classe A", "Ford Fiesta" };
                var listaEstados = new[] { "Disponível", "Alugado", "Em Manutenção" };
                var cores = new[] { "Preto", "Branco", "Cinza", "Azul", "Vermelho" };
                var rand = new Random();

                for (int i = 1; i <= 32; i++)
                {
                    var escolhaCarro = marcasModelos[rand.Next(marcasModelos.Length)].Split(' ');

                    context.Veiculos.Add(new Veiculo
                    {
                        Marca = escolhaCarro[0],
                        Modelo = escolhaCarro.Length > 1 ? string.Join(" ", escolhaCarro.Skip(1)) : "Modelo X",
                        Matricula = $"{(char)rand.Next(65, 91)}{(char)rand.Next(65, 91)}-{rand.Next(10, 99)}-{(char)rand.Next(65, 91)}{(char)rand.Next(65, 91)}", // Ex: AA-12-BB
                  
                        Ano = rand.Next(2019, 2026),
                        Estado = listaEstados[rand.Next(listaEstados.Length)]
                    });
                }
                context.SaveChanges();
            }

            if (!context.RegistosManutencao.Any())
            {
                var primeiroVeiculoId = context.Veiculos.Select(v => v.Id).FirstOrDefault();

                if (primeiroVeiculoId != 0)
                {
                    context.RegistosManutencao.Add(new RegistoManutencao
                    {
                        VeiculoId = primeiroVeiculoId,
                        Data = DateTime.Now.AddDays(-2),
                        Descricao = "Mudança preventiva de óleo e filtros.",
                        Custo = 150.00m,
                        RealizadoPor = "Técnico Administrador"
                    });
                    context.SaveChanges();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RegistoManutencao>()
                .Property(m => m.Custo)
                .HasColumnType("decimal(18,2)");
        }
    }
}