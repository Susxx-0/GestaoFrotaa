using Microsoft.EntityFrameworkCore;
using GestaoDeFrotas.Models;
using System;
using System.Linq;

namespace GestaoDeFrotas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<RegistoManutencao> RegistosManutencao { get; set; }

        public static void SeedData(AppDbContext context)
        {
            // Se não houver veículos, gera as 32 viaturas fakes
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
                        // O ID foi removido aqui para o SQL Server gerir sozinho
                        Marca = escolhaCarro[0],
                        Modelo = escolhaCarro.Length > 1 ? string.Join(" ", escolhaCarro.Skip(1)) : "Modelo X",
                        Matricula = $"{(char)rand.Next(65, 91)}{(char)rand.Next(65, 91)}-{rand.Next(10, 99)}-{(char)rand.Next(65, 91)}{(char)rand.Next(65, 91)}", // Ex: AA-12-BB
                        Cor = cores[rand.Next(cores.Length)],
                        Ano = rand.Next(2019, 2026),
                        Estado = listaEstados[rand.Next(listaEstados.Length)]
                    });
                }
                context.SaveChanges();
            }

            // Se não houver registos de mecânica, cria o primeiro
            if (!context.RegistosManutencao.Any())
            {
                // Vamos buscar o ID do primeiro veículo inserido para garantir que a relação funciona
                var primeiroVeiculoId = context.Veiculos.Select(v => v.Id).FirstOrDefault();

                if (primeiroVeiculoId != 0)
                {
                    context.RegistosManutencao.Add(new RegistoManutencao
                    {
                        // O ID automático também foi removido daqui
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

            // Configura o campo Custo para decimal com 2 casas decimais no SQL Server
            modelBuilder.Entity<RegistoManutencao>()
                .Property(m => m.Custo)
                .HasColumnType("decimal(18,2)");
        }
    }
}