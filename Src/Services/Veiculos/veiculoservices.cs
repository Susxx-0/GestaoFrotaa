using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services.Veiculos
{
    public class VehicleService
    {
        private readonly AppDbContext _context;

        public VehicleService(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // 1. OBTER TODOS (com pesquisa + sorting)
        // ============================
        public async Task<IEnumerable<Veiculo>> ObterTodosAsync(
            string? pesquisa = null,
            string? ordenarPor = null)
        {
            var query = _context.Veiculos.AsQueryable();

            // FILTRO DE PESQUISA
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(v =>
                    v.Marca.Contains(pesquisa) ||
                    v.Modelo.Contains(pesquisa) ||
                    v.Matricula.Contains(pesquisa));
            }

            // SORTING
            query = ordenarPor switch
            {
                "marca" => query.OrderBy(v => v.Marca),
                "modelo" => query.OrderBy(v => v.Modelo),
                "km" => query.OrderByDescending(v => v.KmAtual),
                "estado" => query.OrderBy(v => v.Estado),
                "ipo" => query.OrderBy(v => v.DataProximaIpo),
                _ => query.OrderBy(v => v.Id)
            };

            return await query.AsNoTracking().ToListAsync();
        }

        // ============================
        // 2. OBTER POR ID
        // ============================
        public async Task<Veiculo?> ObterPorIdAsync(int id)
        {
            return await _context.Veiculos.FindAsync(id);
        }

        // ============================
        // 3. CRIAR VEÍCULO
        // ============================
        public async Task<Veiculo> CriarAsync(Veiculo veiculo)
        {
            veiculo.Estado = "Disponível";
            veiculo.EstaAtivo = true;

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();

            return veiculo;
        }

        // ============================
        // 4. ATUALIZAR VEÍCULO
        // ============================
        public async Task<bool> AtualizarAsync(int id, Veiculo dados)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return false;

            veiculo.Marca = dados.Marca;
            veiculo.Modelo = dados.Modelo;
            veiculo.Matricula = dados.Matricula;
            veiculo.KmAtual = dados.KmAtual;
            veiculo.DataProximaIpo = dados.DataProximaIpo;
            veiculo.DataSeguro = dados.DataSeguro;

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // 5. ALTERAR ESTADO DO VEÍCULO
        // ============================
        public async Task<bool> AlterarEstadoAsync(int id, string novoEstado)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return false;

            // Regras de negócio
            if (novoEstado == "Alugado" && veiculo.Estado == "Em Manutenção")
                throw new InvalidOperationException("Veículo em manutenção não pode ser alugado.");

            if (novoEstado == "Alugado" && veiculo.DataProximaIpo < DateTime.Now)
                throw new InvalidOperationException("Veículo com IPO expirada não pode iniciar viagem.");

            if (novoEstado == "Alugado" && veiculo.DataSeguro < DateTime.Now)
                throw new InvalidOperationException("Veículo com seguro expirado não pode iniciar viagem.");

            veiculo.Estado = novoEstado;
            await _context.SaveChangesAsync();

            return true;
        }

        // ============================
        // 6. DESATIVAR VEÍCULO
        // ============================
        public async Task<bool> DesativarAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return false;

            veiculo.EstaAtivo = false;
            veiculo.Estado = "Indisponível";

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // 7. HISTÓRICO DE ALTERAÇÕES
        // ============================
        public async Task RegistarHistoricoAsync(int veiculoId, string acao)
        {
            var historico = new HistoricoVeiculo
            {
                VeiculoId = veiculoId,
                Acao = acao,
                Data = DateTime.Now
            };

            _context.HistoricoVeiculos.Add(historico);
            await _context.SaveChangesAsync();
        }

        // ============================
        // 8. VERIFICAR SE VEÍCULO PODE INICIAR VIAGEM
        // ============================
        public async Task ValidarDisponibilidadeParaViagem(int veiculoId)
        {
            var v = await _context.Veiculos.FindAsync(veiculoId);

            if (v == null)
                throw new InvalidOperationException("Veículo não encontrado.");

            if (!v.EstaAtivo)
                throw new InvalidOperationException("Veículo desativado não pode iniciar viagem.");

            if (v.Estado == "Em Manutenção")
                throw new InvalidOperationException("Veículo em manutenção não pode iniciar viagem.");

            if (v.DataProximaIpo < DateTime.Now)
                throw new InvalidOperationException("IPO expirada — não pode iniciar viagem.");

            if (v.DataSeguro < DateTime.Now)
                throw new InvalidOperationException("Seguro expirado — não pode iniciar viagem.");
        }
    }
}
