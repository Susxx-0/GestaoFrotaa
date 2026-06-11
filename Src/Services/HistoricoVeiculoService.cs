using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class HistoricoVeiculoService
    {
        private readonly AppDbContext _context;

        public HistoricoVeiculoService(AppDbContext context)
        {
            _context = context;
        }

        // Obtém todos os dados cruzados de um veículo específico para gerar os relatórios (Excel/PDF)
        public async Task<object> ObterHistoricoCompletoAsync(int veiculoId)
        {
            var veiculo = await _context.Veiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == veiculoId);

            if (veiculo == null) throw new Exception("Veículo não encontrado.");

            // 1. Histórico de Viagens (Quem levou, quem entregou, horas exatas e duração)
            var viagens = await _context.Viagens
                .Where(v => v.VeiculoId == veiculoId)
                .OrderByDescending(v => v.DataInicio)
                .Select(v => new
                {
                    v.Id,
                    CondutorPrincipal = v.CondutorPrincipalId, // Aqui o frontend/query fará o Join para o nome
                    CondutorSecundario = v.CondutorSecundarioId,
                    v.DataInicio,
                    v.DataFim,
                    DuracaoHoras = v.DataFim.HasValue ? (v.DataFim.Value - v.DataInicio).TotalHours : 0,
                    v.ObservacoesEntrega
                })
                .AsNoTracking()
                .ToListAsync();

            // 2. Histórico de Abastecimentos (Litros, KM e Datas)
            // NOTA: Ajustei para "Posto" e "Data" com base no erro que corrigimos no teu validador!
            var abastecimentos = await _context.Abastecimentos
                .Where(a => a.VeiculoId == veiculoId)
                .OrderByDescending(a => a.Data)
                .Select(a => new
                {
                    a.Id,
                    a.Litros,
                    a.PrecoPorLitro,
                    CustoTotal = a.Litros * a.PrecoPorLitro,
                    KmNoMomento = a.KmAtual,
                    DataRegisto = a.Data,
                    Posto = a.Posto
                })
                .AsNoTracking()
                .ToListAsync();

            // 3. Monta o objeto unificado pronto para ser consumido pelos geradores de PDF/Excel
            var relatorioCompleto = new
            {
                VeiculoMatricula = veiculo.Matricula,
                VeiculoInfo = $"{veiculo.Marca} {veiculo.Modelo} ({veiculo.Ano})",
                HistoricoDeViagens = viagens,
                HistoricoDeAbastecimentos = abastecimentos
            };

            return relatorioCompleto;
        }
    }
}