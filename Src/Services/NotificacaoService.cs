using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Services
{
    public class NotificacoesService
    {
        private readonly AppDbContext _context;
        private readonly AuditoriaService _auditoria;

        public NotificacoesService(AppDbContext context, AuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        // 🔥 Verificar datas a expirar (seguro, inspeção, manutenção)
        public async Task VerificarDatasAsync()
        {
            var veiculos = await _context.Veiculos.ToListAsync();

            foreach (var v in veiculos)
            {
                if (v.ProximaManutencaoData != null &&
                    v.ProximaManutencaoData <= DateTime.Today.AddDays(7))
                {
                    await _auditoria.CriarNotificacaoAsync(
                        $"A manutenção do veículo {v.Matricula} expira em breve.",
                        "Aviso",
                        v.Id
                    );
                }

                if (v.DataInspecao != null &&
                    v.DataInspecao <= DateTime.Today.AddDays(7))
                {
                    await _auditoria.CriarNotificacaoAsync(
                        $"A inspeção do veículo {v.Matricula} expira em breve.",
                        "Aviso",
                        v.Id
                    );
                }

                if (v.DataSeguro != null &&
                    v.DataSeguro <= DateTime.Today.AddDays(7))
                {
                    await _auditoria.CriarNotificacaoAsync(
                        $"O seguro do veículo {v.Matricula} expira em breve.",
                        "Aviso",
                        v.Id
                    );
                }
            }
        }
    }
}
