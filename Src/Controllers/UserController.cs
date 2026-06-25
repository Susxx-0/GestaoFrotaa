using GestoreDeFrotas.Data;
using GestoreDeFrotas.DTOs;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services.OCR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/utilizadores")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR TODOS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // OBTER POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Utilizador não encontrado.");
            return Ok(user);
        }

        // CRIAR UTILIZADOR
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDTO dto)
        {
            var user = new User
            {
                Nome = dto.Nome,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                Ativo = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }


        // EDITAR UTILIZADOR
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User updated)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Utilizador não encontrado.");

            user.Nome = updated.Nome;
            user.Role = updated.Role;
            user.Ativo = updated.Ativo;

            await _context.SaveChangesAsync();
            return Ok(user);
        }



        // DESATIVAR
        [HttpPatch("{id}/desativar")]
        public async Task<IActionResult> Desativar(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Ativo = false;
            await _context.SaveChangesAsync();
            return Ok("Utilizador desativado.");
        }

        // ATIVAR
        [HttpPatch("{id}/ativar")]
        public async Task<IActionResult> Ativar(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Ativo = true;
            await _context.SaveChangesAsync();
            return Ok("Utilizador ativado.");
        }

        [HttpPost("upload-carta")]
        public async Task<IActionResult> UploadCarta(
    [FromForm] IFormFile ficheiro,
    [FromForm] int userId,
    [FromServices] OcrService ocr,
    [FromServices] CartaParserService parser)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Utilizador não encontrado.");

            var pasta = Path.Combine("wwwroot", "cartas");
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            var nomeFicheiro = $"{Guid.NewGuid()}_{ficheiro.FileName}";
            var caminho = Path.Combine(pasta, nomeFicheiro);

            using (var stream = new FileStream(caminho, FileMode.Create))
                await ficheiro.CopyToAsync(stream);

            // OCR
            var texto = ocr.LerTexto(caminho);

            // Parsing
            user.CartaConducaoValidade = parser.ExtrairValidade(texto);
            user.CartaConducaoNumero = parser.ExtrairNumero(texto);
            user.CartaConducaoCategoria = parser.ExtrairCategoria(texto);
            user.CartaConducaoFicheiro = nomeFicheiro;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Carta processada com sucesso.",
                user.CartaConducaoValidade,
                user.CartaConducaoNumero,
                user.CartaConducaoCategoria
            });
        }

    }
}
