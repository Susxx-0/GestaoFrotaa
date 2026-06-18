using GestoreDeFrotas.Models.DTOs.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestoreDeFrotas.Services
{
    public class AuthService
    {
        private readonly string _jwtKey;

        public AuthService(IConfiguration config)
        {
            _jwtKey = config["Jwt:Key"]!;
        }

        public LoginResponse GerarToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var expiracao = DateTime.UtcNow.AddHours(3);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiracao,
                signingCredentials: creds
            );

            return new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraEm = expiracao
            };
        }
    }
}
