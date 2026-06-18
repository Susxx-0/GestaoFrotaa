using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Manutencao
{
    public class MaintenanceService
    {
        private readonly AppDbContext _context;

        public MaintenanceService(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // 1. OBTER TODAS AS MANUTENÇÕES
        // ============================
        public async Task<IEnumerable<RegistoManutencao>> ObterTodasAsync()
        {
            return await _context.RegistosManutencao
                .Include(m => m.Veiculo)
                .OrderByDescending(m => m.Data)
                .AsNoTracking()
                .ToListAsync();
        }

        // ============================
        // 2. OBTER POR ID
        // ============================
        public async Task<RegistoManutencao?> ObterPorIdAsync(int id)
        {
            return await _context.RegistosManutencao
                .Include(m => m.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // ============================
        // 3. CRIAR MANUTENÇÃO
        // ============================
        public async Task<RegistoManutencao> CriarAsync(RegistoManutencao manutencao)
        {
            var veiculo = await _context.Veiculos.FindAsync(manutencao.VeiculoId);
            if (veiculo == null)
                throw new InvalidOperationException("Veículo não encontrado.");

            // Veículo entra em manutenção
            veiculo.Estado = "Em Manutenção";

            _context.RegistosManutencao.Add(manutencao);
            await _context.SaveChangesAsync();

            return manutencao;
        }

        // ============================
        // 4. ATUALIZAR MANUTENÇÃO
        // ============================
        public async Task<bool> AtualizarAsync(int id, RegistoManutencao dados)
        {
            var manutencao = await _context.RegistosManutencao.FindAsync(id);
            if (manutencao == null)
                return false;

            manutencao.Descricao = dados.Descricao;
            manutencao.Custo = dados.Custo;
            manutencao.Data = dados.Data;
            manutencao.RealizadoPor = dados.RealizadoPor;
            manutencao.Km = dados.Km;

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // 5. CONCLUIR MANUTENÇÃO
        // ============================
        public async Task<bool> ConcluirAsync(int id)
        {
            var manutencao = await _context.RegistosManutencao.FindAsync(id);
            if (manutencao == null)
                return false;

            var veiculo = await _context.Veiculos.FindAsync(manutencao.VeiculoId);
            if (veiculo == null)
                return false;

            // Veículo volta a estar disponível
            veiculo.Estado = "Disponível";

            manutencao.DataConclusao = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // 6. MANUTENÇÕES ATRASADAS
        // ============================
        public async Task<IEnumerable<object>> ObterManutencoesAtrasadasAsync()
        {
            var hoje = DateTime.Now.Date;

            var atrasadas = await _context.RegistosManutencao
                .Include(m => m.Veiculo)
                .Where(m => m.DataPrevista != null && m.DataPrevista < hoje && m.DataConclusao == null)
                .OrderBy(m => m.DataPrevista)
                .ToListAsync();

            return atrasadas.Select(m => new
            {
                m.Id,
                m.VeiculoId,
                m.Veiculo.Matricula,
                m.Descricao,
                m.DataPrevista,
                DiasAtraso = (hoje - m.DataPrevista!.Value.Date).Days
            });
        }

        // ============================
        // 7. MANUTENÇÕES PREVISTAS (PRÓXIMOS 30 DIAS)
        // ============================
        public async Task<IEnumerable<object>> ObterManutencoesPrevistasAsync()
        {
            var hoje = DateTime.Now.Date;
            var limite = hoje.AddDays(30);

            var previstas = await _context.RegistosManutencao
                .Include(m => m.Veiculo)
                .Where(m => m.DataPrevista != null && m.DataPrevista >= hoje && m.DataPrevista <= limite)
                .OrderBy(m => m.DataPrevista)
                .ToListAsync();

            return previstas.Select(m => new
            {
                m.Id,
                m.VeiculoId,
                m.Veiculo.Matricula,
                m.Descricao,
                m.DataPrevista,
                DiasRestantes = (m.DataPrevista!.Value.Date - hoje).Days
            });
        }

        
        // 8. BLOQUEAR VEÍCULO SE TIVER MANUTENÇÃO PENDENTE
      
        public async Task ValidarManutencaoAntesDeViagem(int veiculoId)
        {
            var pendente = await _context.RegistosManutencao
                .AnyAsync(m => m.VeiculoId == veiculoId && m.DataConclusao == null);

            if (pendente)
                throw new InvalidOperationException("O veículo possui manutenção pendente e não pode iniciar viagem.");
        }

        // ============================
        // 9. OBTENER Manutencao Estado do Veículo
        // ============================
  public async Task<object> ObterEstadoManutencaoAsync(int veiculoId)
        {
            var pendente = await _context.RegistosManutencao
                .AnyAsync(m => m.VeiculoId == veiculoId && m.DataConclusao == null);

            if (pendente)
                return new { EmManutencao = true, Estado = "Manutenção pendente" };

            return new { EmManutencao = false, Estado = "Sem manutenção pendente" };
        }





    }
}
