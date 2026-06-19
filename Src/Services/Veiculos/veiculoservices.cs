using GestoreDeFrotas.Data;
using GestoreDeFrotas.Dtos.Veiculos;
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

        public async Task<Veiculo> CriarAsync(VeiculoCreateDTO dto)
        {
            var veiculo = new Veiculo
            {
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Matricula = dto.Matricula,
                Ano = dto.Ano,
                CategoriaUsuario = dto.CategoriaUsuario,
                Estado = dto.Estado,
                Cor = dto.Cor,
                KmAtual = dto.KmAtual,
                EstaAtivo = true,
                DataCriacao = DateTime.Now
            };

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();

            return veiculo;
        }

        public async Task<Veiculo?> AtualizarAsync(int id, VeiculoUpdateDTO dto)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return null;

            veiculo.Marca = dto.Marca;
            veiculo.Modelo = dto.Modelo;
            veiculo.Matricula = dto.Matricula;
            veiculo.Ano = dto.Ano;
            veiculo.CategoriaUsuario = dto.CategoriaUsuario;
            veiculo.Estado = dto.Estado;
            veiculo.Cor = dto.Cor;
            veiculo.KmAtual = dto.KmAtual;

            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<bool> AlterarEstadoAsync(int id, string novoEstado)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return false;

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
    }
}
