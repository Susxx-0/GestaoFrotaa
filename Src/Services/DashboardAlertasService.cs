using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services
{
    public class DashboardAlertasService
    {
        private readonly AppDbContext _context;

        public DashboardAlertasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterAlertasAsync()
        {
            var hoje = DateTime.Now;

            // Carregar dados
            var abastecimentos = await _context.Abastecimentos
                .Include(a => a.Veiculo)
                .OrderBy(a => a.VeiculoId)
                .ThenBy(a => a.Data)
                .ToListAsync();

            var veiculos = await _context.Veiculos.ToListAsync();

            var alertas = new List<object>();

            // .Consumo anormal
            foreach (var ab in abastecimentos.Where(a => a.ConsumoMedio > 12))
            {
                alertas.Add(new
                {
                    Tipo = "Consumo Anormal",
                    Mensagem = $"O veículo {ab.Veiculo.Marca} {ab.Veiculo.Modelo} ({ab.Veiculo.Matricula}) apresentou consumo de {ab.ConsumoMedio} L/100km.",
                    Data = ab.Data
                });
            }

            //  KM incoerentes
            var abastecimentosPorVeiculo = abastecimentos
                .GroupBy(a => a.VeiculoId);

            foreach (var grupo in abastecimentosPorVeiculo)
            {
                var lista = grupo.OrderBy(a => a.Data).ToList();

                for (int i = 1; i < lista.Count; i++)
                {
                    if (lista[i].KmAtual < lista[i - 1].KmAtual)
                    {
                        alertas.Add(new
                        {
                            Tipo = "KM Incoerente",
                            Mensagem = $"O veículo {lista[i].Veiculo.Marca} {lista[i].Veiculo.Modelo} ({lista[i].Veiculo.Matricula}) registou KM inferior ao anterior.",
                            Data = lista[i].Data
                        });
                    }
                }
            }

            //  Veículos parados há mais de 30 dias
            foreach (var v in veiculos)
            {
                var ultimoAb = abastecimentos
                    .Where(a => a.VeiculoId == v.Id)
                    .OrderByDescending(a => a.Data)
                    .FirstOrDefault();

                if (ultimoAb != null)
                {
                    var dias = (hoje - ultimoAb.Data).TotalDays;

                    if (dias > 30)
                    {
                        alertas.Add(new
                        {
                            Tipo = "Veículo Parado",
                            Mensagem = $"O veículo {v.Marca} {v.Modelo} ({v.Matricula}) está parado há {Math.Round(dias)} dias.",
                            Data = ultimoAb.Data
                        });
                    }
                }
            }

            // Manutenção atrasada / próxima
            foreach (var v in veiculos)
            {
                if (v.IntervaloManutencaoKm == 0) continue;

                int kmDesdeUltima = v.UltimaManutencaoKm.HasValue
                    ? v.KmAtual - v.UltimaManutencaoKm.Value
                    : v.KmAtual;


                double percent = (((double)kmDesdeUltima / v.IntervaloManutencaoKm) * 100) ?? 0.0;

                if (percent >= 120)
                {
                    alertas.Add(new
                    {
                        Tipo = "Manutenção Atrasada",
                        Mensagem = $"O veículo {v.Marca} {v.Modelo} ({v.Matricula}) ultrapassou o limite de manutenção.",
                        Percentagem = Math.Round(percent, 1),
                        Data = hoje
                    });
                }
                else if (percent >= 90)
                {
                    alertas.Add(new
                    {
                        Tipo = "Manutenção Próxima",
                        Mensagem = $"O veículo {v.Marca} {v.Modelo} ({v.Matricula}) está próximo da manutenção.",
                        Percentagem = Math.Round(percent, 1),
                        Data = hoje
                    });
                }
            }

            return new
            {
                TotalAlertas = alertas.Count,
                Alertas = alertas
            };
        }
    }
}