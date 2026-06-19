using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestoreDeFrotas.Models;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using GestoreDeFrotas.Services.Manutencao;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/maintenance")]
    [Authorize]
    public class ManutencaoController : ControllerBase
    {
        private readonly MaintenanceService _maintenanceService;
        private readonly IValidator<RegistoManutencao> _validator;

        public ManutencaoController(MaintenanceService maintenanceService, IValidator<RegistoManutencao> validator)
        {
            _maintenanceService = maintenanceService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var registos = await _maintenanceService.ObterTodasAsync();
            return Ok(registos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegistoManutencao registo)
        {
            var validationResult = await _validator.ValidateAsync(registo);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            await _maintenanceService.CriarAsync(registo);
            return Ok(registo);
        }

        [HttpGet("estado/{veiculoId}")]
        public async Task<IActionResult> Estado(int veiculoId)
        {
            var estado = await _maintenanceService.ObterEstadoManutencaoAsync(veiculoId);
            return Ok(estado);
        }

        
            
        












    }
}
