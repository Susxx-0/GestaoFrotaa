using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Services
{
    public class VeiculosService
    {
        private readonly AppDbContext _context;
        private readonly AuditoriaService _auditoria;

        public VeiculosService(AppDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<List<Veiculo>> ObterTodosAtivosAsync(string? ordenar)
        {
            var q = _context.Veiculos.Where(v => v.EstaAtivo);

            q = ordenar?.ToLower() switch
            {
                "marca" => q.OrderBy(v => v.Marca),
                "km" => q.OrderByDescending(v => v.KmAtual),
                "ano" => q.OrderByDescending(v => v.Ano),
                _ => q.OrderBy(v => v.Id)
            };

            return await q.ToListAsync();
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
            v.Estado = dados.Estado;
            v.KmAtual = dados.KmAtual;

            v.UltimaManutencaoKm = dados.UltimaManutencaoKm;
            v.UltimaManutencaoData = dados.UltimaManutencaoData;

            v.ProximaManutencaoKm = dados.ProximaManutencaoKm;
            v.ProximaManutencaoData = dados.ProximaManutencaoData;

            v.DataInspecao = dados.DataInspecao;
            v.DataSeguro = dados.DataSeguro;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Veiculo>> ObterManutencaoAtrasadaAsync()
        {
            return await _context.Veiculos
                .Where(v =>
                    (v.ProximaManutencaoKm != null && v.KmAtual >= v.ProximaManutencaoKm) ||
                    (v.ProximaManutencaoData != null && v.ProximaManutencaoData <= DateTime.Today)
                )
                .ToListAsync();
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
