using GestoreDeFrotas.Models.DTOs.Auth;
using GestoreDeFrotas.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestoreDeFrotas.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            
            var utilizadores = new Dictionary<string, string>
            {
                { "admin", "Admin" },
                { "gerente", "Gerente" },
                { "tecnico", "Tecnico" },
                { "utilizador", "Utilizador" }
            };

            if (!utilizadores.ContainsKey(request.Username) || request.Password != "1234")
                return Unauthorized("Credenciais inválidas");

            var role = utilizadores[request.Username];

            var token = _auth.GerarToken(request.Username, role);
            return Ok(token);
        }
    }
}
