using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class ManutencaoService
    {
        private readonly AppDbContext _context;

        public ManutencaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegistoManutencao>> ObterTodosAsync()
        {
            return await _context.RegistosManutencao.ToListAsync();
        }

        public async Task AdicionarAsync(RegistoManutencao registo)
        {
            var veiculo = await _context.Veiculos.FindAsync(registo.VeiculoId);
            if (veiculo == null)
                throw new Exception("Veículo não encontrado.");

            veiculo.UltimaManutencaoKm = registo.Km;
            veiculo.UltimaManutencaoData = registo.Data;

            _context.RegistosManutencao.Add(registo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> ObterEstadoManutencaoAsync()
        {
            var veiculos = await _context.Veiculos.ToListAsync();
            var lista = new List<object>();

            foreach (var v in veiculos)
            {
                int kmDesdeUltima = v.UltimaManutencaoKm.HasValue
                    ? v.KmAtual - v.UltimaManutencaoKm.Value
                    : v.KmAtual;

               
                int intervaloKm = v.IntervaloManutencaoKm ?? 150000;

                if (intervaloKm <= 0)
                {
                    intervaloKm = 150000;
                }

             
                double percent = ((double)kmDesdeUltima / intervaloKm) * 100;

                string estado =
                    percent >= 120 ? "Atrasada" :
                    percent >= 90 ? "Próxima" :
                    "OK";

                lista.Add(new
                {
                    v.Id,
                    v.Marca,
                    v.Modelo,
                    v.Matricula,
                    KmDesdeUltima = kmDesdeUltima,
                    Percentagem = Math.Round(percent, 1),
                    Estado = estado
                });
            }

            return lista;
        }
    }
}