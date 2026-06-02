using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestaoDeFrotas.Models;
using GestaoDeFrotas.Services;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/maintenance")]
    [Authorize]
    public class ManutencaoController : ControllerBase // Isto é obrigatório!
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
            if (registo == null) return BadRequest("Dados inválidos.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _manutencaoService.AdicionarAsync(registo);
                return Ok(registo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}