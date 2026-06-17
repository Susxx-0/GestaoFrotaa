using GestoreDeFrotas.Models;
using System.Linq;

namespace GestoreDeFrotas.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Veiculos.Any()) return;

            var v1 = new Veiculo { Marca = "Renault", Modelo = "Clio", Matricula = "AA00XX", Ano = 2022, Estado = "Disponível", CategoriaUsuario = "Empresa", EstaAtivo = true, KmAtual = 10000, UltimaManutencaoKm = 10000 };
            var v2 = new Veiculo { Marca = "BMW", Modelo = "320d", Matricula = "99ZZ11", Ano = 2021, Estado = "Disponível", CategoriaUsuario = "Empresa", EstaAtivo = true, KmAtual = 30000, UltimaManutencaoKm = 30000 };

            context.Veiculos.AddRange(v1, v2);
            context.SaveChanges();
        }
    }
}