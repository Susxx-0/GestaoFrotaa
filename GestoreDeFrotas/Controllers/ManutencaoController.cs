using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Models;
using GestaoDeFrotas.Services;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/maintenance")]
    [Authorize]
    public class ManutencaoController : ControllerBase
    {
        private readonly ManutencaoService _manutencaoService;

        public ManutencaoController(ManutencaoService manutencaoService)
        {
            _manutencaoService = manutencaoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> GetAll()
        {
            var registos = await _manutencaoService.ObterTodosAsync();
            return Ok(registos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Tecnico")]
        public async Task<IActionResult> Create([FromBody] RegistoManutencao registo)
        {
            await _manutencaoService.AdicionarAsync(registo);
            return Ok(registo);
        }

        // 🔥 Endpoint de manutenção inteligente
        [HttpGet("estado")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> Estado()
        {
            var estado = await _manutencaoService.ObterEstadoManutencaoAsync();
            return Ok(estado);
        }
    }
}
