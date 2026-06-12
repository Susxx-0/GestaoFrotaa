using GestoreDeFrotas.Models;
using System;
using System.Linq;

namespace GestoreDeFrotas.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Veiculos.Any())
            {
                context.Veiculos.AddRange(
                    new Veiculo { Marca = "Renault", Modelo = "Clio", Matricula = "AA00XX", Ano = 2022, Cor = "Cinzento", Estado = "Disponível", TecnicoResponsavelId = "tec-01" },
                    new Veiculo { Marca = "Peugeot", Modelo = "Partner", Matricula = "BB11YY", Ano = 2021, Cor = "Branco", Estado = "Em uso", TecnicoResponsavelId = "tec-02" },
                    new Veiculo { Marca = "Tesla", Modelo = "Model 3", Matricula = "CC22ZZ", Ano = 2024, Cor = "Preto", Estado = "Disponível", TecnicoResponsavelId = "tec-01" },
                    new Veiculo { Marca = "BMW", Modelo = "116d", Matricula = "DD33WW", Ano = 2020, Cor = "Azul", Estado = "Disponível", TecnicoResponsavelId = "tec-03" }
                );
                context.SaveChanges();
            }

            if (!context.Viagens.Any())
            {
                var veiculoEmUso = context.Veiculos.FirstOrDefault(v => v.Matricula == "BB11YY");
                var veiculoDisponivel = context.Veiculos.FirstOrDefault(v => v.Matricula == "AA-00XX");

                if (veiculoEmUso != null && veiculoDisponivel != null)
                {
                    context.Viagens.AddRange(
                        new Viagem
                        {
                            VeiculoId = veiculoDisponivel.Id,
                            CondutorPrincipalId = "condutor-antigo",
                            DataInicio = DateTime.Now.AddDays(-2),
                            DataLimitePrevista = DateTime.Now.AddDays(-2).AddHours(4),
                            DataFim = DateTime.Now.AddDays(-2).AddHours(3),
                            EstaAtiva = false,
                            ObservacoesEntrega = "Barulho metalico na roda dianteira esquerda ao travar."
                        },
                        new Viagem
                        {
                            VeiculoId = veiculoEmUso.Id,
                            CondutorPrincipalId = "condutor-atrasado",
                            DataInicio = DateTime.Now.AddHours(-8),
                            DataLimitePrevista = DateTime.Now.AddHours(-4),
                            EstaAtiva = true,
                            PediuProrrogacao = false
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}