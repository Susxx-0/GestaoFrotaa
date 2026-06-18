using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();

            if (!context.Veiculos.Any())
            {
                context.Veiculos.AddRange(
                    new Veiculo
                    {
                        Marca = "Renault",
                        Modelo = "Clio",
                        Matricula = "AA-11-CC",
                        Ano = 2018,
                        KmAtual = 120000,
                        UltimaManutencaoKm = 110000,
                        UltimaManutencaoData = DateTime.Now.AddMonths(-6),
                        ProximaManutencaoKm = 130000,
                        ProximaManutencaoData = DateTime.Now.AddMonths(6),
                        DataInspecao = DateTime.Now.AddMonths(2),
                        DataSeguro = DateTime.Now.AddMonths(3),
                        EstaAtivo = true
                    },
                    new Veiculo
                    {
                        Marca = "Volkswagen",
                        Modelo = "Golf",
                        Matricula = "BB-22-DD",
                        Ano = 2020,
                        KmAtual = 80000,
                        UltimaManutencaoKm = 70000,
                        UltimaManutencaoData = DateTime.Now.AddMonths(-4),
                        ProximaManutencaoKm = 90000,
                        ProximaManutencaoData = DateTime.Now.AddMonths(8),
                        DataInspecao = DateTime.Now.AddMonths(1),
                        DataSeguro = DateTime.Now.AddMonths(5),
                        EstaAtivo = true
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
