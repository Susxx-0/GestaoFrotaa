using GestoreDeFrotas.Data;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Dashboard
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1) RESUMO GERAL
        // ============================================================
        public async Task<object> ObterResumoGeralAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            decimal totalCombustivel = await _context.Abastecimentos
                .SumAsync(a => (decimal?)a.CustoTotal) ?? 0;

            decimal totalCombustivelMes = await _context.Abastecimentos
                .Where(a => a.Data >= inicioMes)
                .SumAsync(a => (decimal?)a.CustoTotal) ?? 0;

            decimal totalManutencoes = await _context.RegistosManutencao
                .SumAsync(m => (decimal?)m.Custo) ?? 0;

            decimal totalManutencoesMes = await _context.RegistosManutencao
                .Where(m => m.Data >= inicioMes)
                .SumAsync(m => (decimal?)m.Custo) ?? 0;

            decimal mediaKm = await _context.Veiculos.AnyAsync()
                ? await _context.Veiculos.AverageAsync(v => (decimal?)v.KmAtual) ?? 0
                : 0;

            int veiculosAtribuidos = await _context.AtribuicoesVeiculo
                .CountAsync(a => a.Ativo);

            int veiculosSemCondutor = await _context.Veiculos
                .CountAsync(v => !_context.AtribuicoesVeiculo
                    .Any(a => a.VeiculoId == v.Id && a.Ativo));

            return new
            {
                TotalVeiculos = await _context.Veiculos.CountAsync(),
                Disponiveis = await _context.Veiculos.CountAsync(v => v.Estado == "Disponível"),
                Alugados = await _context.Veiculos.CountAsync(v => v.Estado == "Alugado"),
                EmManutencao = await _context.Veiculos.CountAsync(v => v.Estado == "Em Manutenção"),

                TotalAbastecimentos = await _context.Abastecimentos.CountAsync(),
                TotalGastoCombustivel = totalCombustivel,
                TotalGastoCombustivelMes = totalCombustivelMes,

                NumeroTotalManutencoes = await _context.RegistosManutencao.CountAsync(),
                TotalGastoManutencoes = totalManutencoes,
                TotalGastoManutencoesMes = totalManutencoesMes,

                TotalDocumentos = await _context.DocumentosVeiculos.CountAsync(),
                DocumentosExpirados = await _context.DocumentosVeiculos
                    .CountAsync(d => d.DataValidade != null && d.DataValidade < hoje),

                MediaKmAtual = mediaKm,

                // NOVOS CAMPOS
                VeiculosAtribuidos = veiculosAtribuidos,
                VeiculosSemCondutor = veiculosSemCondutor
            };
        }

        // ============================================================
        // 2) VEÍCULOS EM USO (VIAGENS ATIVAS)
        // ============================================================
        public async Task<object> ObterVeiculosEmUsoDashboardAsync()
        {
            var agora = DateTime.Now;

            var dados = await _context.Viagens
                .Where(v => v.EstaAtiva)
                .Join(_context.Veiculos,
                    viagem => viagem.VeiculoId,
                    veiculo => veiculo.Id,
                    (viagem, veiculo) => new { viagem, veiculo })
                .OrderBy(x => x.viagem.DataInicio)
                .Select(x => new
                {
                    x.viagem.Id,
                    x.viagem.VeiculoId,
                    x.veiculo.Matricula,
                    Modelo = x.veiculo.Marca + " " + x.veiculo.Modelo,
                    x.viagem.CondutorPrincipalId,
                    x.viagem.CondutorSecundarioId,
                    x.viagem.DataInicio,
                    x.viagem.DataLimitePrevista,
                    x.viagem.PediuProrrogacao
                })
                .AsNoTracking()
                .ToListAsync();

            return dados.Select(v => new
            {
                v.Id,
                v.VeiculoId,
                v.Matricula,
                v.Modelo,
                v.CondutorPrincipalId,
                v.CondutorSecundarioId,
                v.DataInicio,
                v.DataLimitePrevista,
                TempoUsoHoras = (agora - v.DataInicio).TotalHours,
                ExcedeuTempoLimite = agora > v.DataLimitePrevista,
                v.PediuProrrogacao
            });
        }

        // ============================================================
        // 3) ALERTAS IPO
        // ============================================================
        public async Task<object> ObterAlertasIpoDashboardAsync()
        {
            var hoje = DateTime.Now.Date;
            var limite = hoje.AddDays(30);

            var veiculos = await _context.Veiculos
                .Where(v => v.EstaAtivo && v.DataProximaIpo != null && v.DataProximaIpo <= limite)
                .OrderBy(v => v.DataProximaIpo)
                .Select(v => new
                {
                    v.Id,
                    v.Matricula,
                    Descricao = v.Marca + " " + v.Modelo,
                    v.Estado,
                    v.DataProximaIpo
                })
                .AsNoTracking()
                .ToListAsync();

            return veiculos.Select(v => new
            {
                v.Id,
                v.Matricula,
                v.Descricao,
                v.Estado,
                v.DataProximaIpo,
                DiasRestantes = v.DataProximaIpo.HasValue
                    ? (v.DataProximaIpo.Value.Date - hoje).Days
                    : 0,
                Alerta = v.DataProximaIpo < hoje
                    ? "IPO EXPIRADA"
                    : "CRÍTICO (Menos de 30 dias)"
            });
        }
    }
}
