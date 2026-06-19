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

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync(string? pesquisa, string? ordenarPor)
        {
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(v =>
                    v.Marca.Contains(pesquisa) ||
                    v.Modelo.Contains(pesquisa) ||
                    v.Matricula.Contains(pesquisa));
            }

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

        public async Task<Veiculo?> ObterPorIdAsync(int id)
        {
            return await _context.Veiculos.FindAsync(id);
        }

        public async Task<Veiculo> CriarAsync(Veiculo veiculo)
        {
            veiculo.Estado = "Disponível";
            veiculo.EstaAtivo = true;

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();

            return veiculo;
        }

        public async Task<Veiculo?> AtualizarAsync(int id, Veiculo dados)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return null;

            veiculo.Marca = dados.Marca;
            veiculo.Modelo = dados.Modelo;
            veiculo.Matricula = dados.Matricula;
            veiculo.KmAtual = dados.KmAtual;
            veiculo.DataProximaIpo = dados.DataProximaIpo;
            veiculo.DataSeguro = dados.DataSeguro;

            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<bool> AlterarEstadoAsync(int id, string novoEstado)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return false;

            if (novoEstado == "Alugado" && veiculo.Estado == "Em Manutenção")
                throw new InvalidOperationException("Veículo em manutenção não pode ser alugado.");

            if (novoEstado == "Alugado" && veiculo.DataProximaIpo < DateTime.Now)
                throw new InvalidOperationException("IPO expirada.");

            if (novoEstado == "Alugado" && veiculo.DataSeguro < DateTime.Now)
                throw new InvalidOperationException("Seguro expirado.");

            veiculo.Estado = novoEstado;
            await _context.SaveChangesAsync();

            return true;
        }

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

        public async Task<bool> AtivarAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return false;

            veiculo.EstaAtivo = true;
            veiculo.Estado = "Disponível";

            await _context.SaveChangesAsync();
            return true;
        }

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

        public async Task ValidarDisponibilidadeParaViagem(int veiculoId)
        {
            var v = await _context.Veiculos.FindAsync(veiculoId);

            if (v == null)
                throw new InvalidOperationException("Veículo não encontrado.");

            if (!v.EstaAtivo)
                throw new InvalidOperationException("Veículo desativado.");

            if (v.Estado == "Em Manutenção")
                throw new InvalidOperationException("Veículo em manutenção.");

            if (v.DataProximaIpo < DateTime.Now)
                throw new InvalidOperationException("IPO expirada.");

            if (v.DataSeguro < DateTime.Now)
                throw new InvalidOperationException("Seguro expirado.");
        }
    }
}
