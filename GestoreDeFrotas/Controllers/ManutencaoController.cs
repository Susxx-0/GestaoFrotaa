using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using System;
using System.Linq;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/maintenance")] 
    [Authorize]
    public class ManutencaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManutencaoController(AppDbContext context) => _context = context;

        // GET /maintenance -> Todos veem o histórico
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Técnico,Visualizador")]
        public IActionResult ObterHistorico() => Ok(_context.RegistosManutencao.ToList());

        // POST /maintenance -> Admin, Gerente e Técnico criam registos
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Técnico")]
        public IActionResult CriarRegisto([FromBody] RegistoManutencao registo)
        {
            var nomeUtilizador = User.Identity?.Name ?? "Técnico do Portal";
            registo.RealizadoPor = $"Registado por: {nomeUtilizador}";
            registo.Data = DateTime.Now;

            _context.RegistosManutencao.Add(registo);
            _context.SaveChanges();
            return Created(string.Empty, registo);
        }
    }
}