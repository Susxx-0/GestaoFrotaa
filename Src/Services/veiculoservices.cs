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

        public VeiculosService(AppDbContext context)
        {
            _context = context;
        }

        // LISTA NORMAL: Apenas mostra veículos ativos (para condutores e técnicos no dia a dia)
        public async Task<List<Veiculo>> ObterTodosAtivosAsync()
        {
            return await _context.Veiculos
                .Where(v => v.EstaAtivo)
                .AsNoTracking()
                .ToListAsync();
        }

        // ARQUIVO (Apenas Admins): Mostra os veículos desativados/antigos
        public async Task<List<Veiculo>> ObterArquivoInativosAsync()
        {
            return await _context.Veiculos
                .Where(v => !v.EstaAtivo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AdicionarAsync(Veiculo veiculo)
        {
            if (veiculo == null) throw new ArgumentNullException(nameof(veiculo));

       
            veiculo.Id = 0;

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) throw new Exception("Veículo não encontrado.");

            // Em vez de apagar fisicamente, altera o estado para inativo (Soft Delete seguro)
            veiculo.EstaAtivo = false;
            await _context.SaveChangesAsync();
        }

        // ALTERAR ESTADO DE ATIVO (Arquivar / Restaurar)
        public async Task AlterarEstadoAtivoAsync(int veiculoId, bool status)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null) throw new Exception("Veículo não encontrado.");

            veiculo.EstaAtivo = status;
            await _context.SaveChangesAsync();
        }
    }
}