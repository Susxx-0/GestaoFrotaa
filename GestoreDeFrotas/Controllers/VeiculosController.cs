using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaoDeFrotas.Data;
using GestaoDeFrotas.Models;
using System.Linq;

namespace GestaoDeFrotas.Controllers
{
    [ApiController]
    [Route("api/vehicles")] 
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeiculosController(AppDbContext context) => _context = context;

        // GET /vehicles -> Todos os cargos podem ver
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente,Técnico,Visualizador")]
        public IActionResult ObterTodos() => Ok(_context.Veiculos.ToList());

        // POST /vehicles -> Apenas Admin e Gerente criam viaturas
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Criar([FromBody] Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterTodos), new { id = veiculo.Id }, veiculo);
        }

        // PUT /vehicles/{id} -> Apenas Admin e Gerente editam viaturas
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Atualizar(int id, [FromBody] Veiculo veiculoAtualizado)
        {
            var existente = _context.Veiculos.Find(id);
            if (existente == null) return NotFound("Veículo não encontrado.");

            existente.Marca = veiculoAtualizado.Marca;
            existente.Modelo = veiculoAtualizado.Modelo;
            existente.Matricula = veiculoAtualizado.Matricula;
            existente.Cor = veiculoAtualizado.Cor;
            existente.Ano = veiculoAtualizado.Ano;
            existente.Estado = veiculoAtualizado.Estado;

            _context.SaveChanges();
            return NoContent();
        }

        // DELETE /vehicles/{id} -> SÓ O ADMIN APAGA!
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Apagar(int id)
        {
            var veiculo = _context.Veiculos.Find(id);
            if (veiculo == null) return NotFound("Veículo não encontrado.");

            _context.Veiculos.Remove(veiculo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}