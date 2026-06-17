using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
                _ => query.OrderBy(v => v.Id) // Ordenação por defeito
            };

            return await query.ToListAsync();
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id) =>
            await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id && v.EstaAtivo);

        public async Task CriarAsync(Veiculo v)
        {
            _context.Veiculos.Add(v);
            await _context.SaveChangesAsync();
        }

        // ADICIONADO: Atualização completa de dados do veículo
        public async Task<bool> AtualizarAsync(int id, Veiculo dadosAtualizados)
        {
            var veiculo = await ObterPorIdAsync(id);
            if (veiculo == null) return false;

            veiculo.Marca = dadosAtualizados.Marca;
            veiculo.Modelo = dadosAtualizados.Modelo;
            veiculo.Matricula = dadosAtualizados.Matricula;
            veiculo.Ano = dadosAtualizados.Ano;
            veiculo.CategoriaUsuario = dadosAtualizados.CategoriaUsuario;

            // Só permite alterar o quilometrismo manualmente se for superior ao atual
            if (dadosAtualizados.KmAtual >= veiculo.KmAtual)
            {
                veiculo.KmAtual = dadosAtualizados.KmAtual;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarEstadoManutencaoAsync(int id, bool colocarEmManutencao)
        {
            var veiculo = await ObterPorIdAsync(id);
            if (veiculo == null) return false;

            if (colocarEmManutencao)
            {
                if (veiculo.Estado == "Em uso")
                    throw new Exception("Não é possível enviar para manutenção um veículo que está em viagem ativa.");

                veiculo.Estado = "Em Manutenção";
            }
            else
            {
                veiculo.Estado = "Disponível";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task EliminarAsync(int id)
        {
            var veiculo = await ObterPorIdAsync(id);
            if (veiculo != null)
            {
                veiculo.EstaAtivo = false; // Soft Delete protegido
                await _context.SaveChangesAsync();
            }
        }
    }
}