using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class VeiculosService
    {
        private readonly AppDbContext _context;
        public VeiculosService(AppDbContext context) => _context = context;

        public async Task<List<Veiculo>> ObterTodosAtivosAsync(string? criterioOrdenacao = null)
        {
            var query = _context.Veiculos.Where(v => v.EstaAtivo);

            query = criterioOrdenacao?.ToLower() switch
            {
                "marca" => query.OrderBy(v => v.Marca).ThenBy(v => v.Modelo),
                "km" => query.OrderByDescending(v => v.KmAtual),
                "ano" => query.OrderByDescending(v => v.Ano),
                "estado" => query.OrderBy(v => v.Estado),
                _ => query.OrderBy(v => v.Id)
            };

            return await query.ToListAsync();
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id) =>
            await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id && v.EstaAtivo);

        public async Task<Veiculo> CriarAsync(Veiculo v)
        {
            _context.Veiculos.Add(v);
            await _context.SaveChangesAsync();
            return v;
        }

        public async Task<bool> AtualizarAsync(int id, Veiculo dados)
        {
            var v = await ObterPorIdAsync(id);
            if (v == null) return false;

            v.Marca = dados.Marca;
            v.Modelo = dados.Modelo;
            v.Matricula = dados.Matricula;
            v.Ano = dados.Ano;
            v.CategoriaUsuario = dados.CategoriaUsuario;

            if (dados.KmAtual >= v.KmAtual)
                v.KmAtual = dados.KmAtual;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarEstadoManutencaoAsync(int id, bool emManutencao)
        {
            var v = await ObterPorIdAsync(id);
            if (v == null) return false;

            if (emManutencao && v.Estado == "Em uso")
                throw new System.Exception("Não é possível enviar para manutenção um veículo que está em viagem ativa.");

            v.Estado = emManutencao ? "Em Manutenção" : "Disponível";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var v = await _context.Veiculos.FindAsync(id);
            if (v == null) return false;

            v.EstaAtivo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
