using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/maintenance")]
    [Authorize]
    public class ManutencaoController : ControllerBase
    {
        private readonly ManutencaoService _manutencaoService;
        private readonly IValidator<RegistoManutencao> _validator;

        public ManutencaoController(ManutencaoService manutencaoService, IValidator<RegistoManutencao> validator)
        {
            _manutencaoService = manutencaoService;
            _validator = validator;
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
            var validationResult = await _validator.ValidateAsync(registo);

            if (!validationResult.IsValid)
            {
                // Retorna erro 400 com a lista detalhada de falhas
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            await _manutencaoService.AdicionarAsync(registo);
            return Ok(registo);
        }

        [HttpGet("estado")]
        [Authorize(Roles = "Admin,Gerente,Tecnico,Visualizador")]
        public async Task<IActionResult> Estado()
        {
            try
            {
                var estado = await _manutencaoService.ObterEstadoManutencaoAsync();
                return Ok(estado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro interno ao calcular o estado de manutenção.",
                    detalhe = ex.Message
                });
            }
        }
    }
}