using GestoreDeFrotas.Data;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> ObterResumoGeralAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var totalVeiculos = await _context.Veiculos.CountAsync();

            var resumo = new
            {
                TotalVeiculos = totalVeiculos,
                Disponiveis = await _context.Veiculos.CountAsync(v => v.Estado == "Disponível"),
                Alugados = await _context.Veiculos.CountAsync(v => v.Estado == "Alugado"),
                EmManutencao = await _context.Veiculos.CountAsync(v => v.Estado == "Em Manutenção"),

                TotalAbastecimentos = await _context.Abastecimentos.CountAsync(),
                TotalGastoCombustivel = await _context.Abastecimentos.SumAsync(a => (double?)a.CustoTotal) ?? 0,
                TotalGastoCombustivelMes = await _context.Abastecimentos
                    .Where(a => a.Data >= inicioMes)
                    .SumAsync(a => (double?)a.CustoTotal) ?? 0,

                NumeroTotalManutencoes = await _context.RegistosManutencao.CountAsync(),
                TotalGastoManutencoes = await _context.RegistosManutencao.SumAsync(m => (decimal?)m.Custo) ?? 0,
                TotalGastoManutencoesMes = await _context.RegistosManutencao
                    .Where(m => m.Data >= inicioMes)
                    .SumAsync(m => (decimal?)m.Custo) ?? 0,

                TotalDocumentos = await _context.DocumentosVeiculos.CountAsync(),
                DocumentosExpirados = await _context.DocumentosVeiculos
                    .CountAsync(d => d.DataValidade != null && d.DataValidade < hoje),

                MediaKmAtual = await _context.Veiculos.AnyAsync()
                    ? await _context.Veiculos.AverageAsync(v => (double?)v.KmAtual) ?? 0
                    : 0
            };

            return resumo;
        }

        public async Task<object> ObterVeiculosEmUsoDashboardAsync()
        {
            var agora = DateTime.Now;

            var dadosViagens = await _context.Viagens
                .Where(v => v.EstaAtiva)
                .Join(_context.Veiculos,
                    viagem => viagem.VeiculoId,
                    veiculo => veiculo.Id,
                    (viagem, veiculo) => new { viagem, veiculo })
                .OrderBy(x => x.viagem.DataInicio)
                .Select(x => new
                {
                    ViagemId = x.viagem.Id,
                    VeiculoId = x.viagem.VeiculoId,
                    Matricula = x.veiculo.Matricula,
                    ModeloInfo = x.veiculo.Marca + " " + x.veiculo.Modelo,
                    CondutorPrincipal = x.viagem.CondutorPrincipalId,
                    CondutorSecundario = x.viagem.CondutorSecundarioId,
                    DataInicioReal = x.viagem.DataInicio,
                    DataPrazoPrevisto = x.viagem.DataLimitePrevista,
                    PediuProrrogacao = x.viagem.PediuProrrogacao
                })
                .AsNoTracking()
                .ToListAsync();

            var resultadoFinal = dadosViagens.Select(v => new
            {
                v.ViagemId,
                v.VeiculoId,
                v.Matricula,
                v.ModeloInfo,
                v.CondutorPrincipal,
                v.CondutorSecundario,
                v.DataInicioReal,
                v.DataPrazoPrevisto,
                TempoUsoHoras = (agora - v.DataInicioReal).TotalHours,
                ExcedeuTempoLimite = agora > v.DataPrazoPrevisto,
                v.PediuProrrogacao
            }).ToList();

            return resultadoFinal;
        }

        public async Task<object> ObterAlertasIpoDashboardAsync()
        {
            var hoje = DateTime.Now.Date;
            var dataLimiteAlerta = hoje.AddDays(30);

            var veiculosComIpo = await _context.Veiculos
                .Where(v => v.EstaAtivo && v.DataProximaIpo != null && v.DataProximaIpo <= dataLimiteAlerta)
                .OrderBy(v => v.DataProximaIpo)
                .Select(v => new
                {
                    VeiculoId = v.Id,
                    v.Matricula,
                    DescricaoVeiculo = v.Marca + " " + v.Modelo,
                    v.Estado,
                    DataLimiteIpo = v.DataProximaIpo
                })
                .AsNoTracking()
                .ToListAsync();

            var resultadoFinal = veiculosComIpo.Select(v => new
            {
                v.VeiculoId,
                v.Matricula,
                v.DescricaoVeiculo,
                v.Estado,
                v.DataLimiteIpo,
                DiasRestantes = v.DataLimiteIpo.HasValue ? (v.DataLimiteIpo.Value.Date - hoje).Days : 0,
                AlertaStatus = v.DataLimiteIpo.HasValue && v.DataLimiteIpo.Value.Date < hoje ? "IPO EXPIRADA" : "CRÍTICO (Menos de 30 dias)"
            }).ToList();

            return resultadoFinal;
        }
    }
}
