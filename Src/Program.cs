using GestoreDeFrotas.Data;
using GestoreDeFrotas.Services;
using GestoreDeFrotas.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Base de Dados
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Serviços
builder.Services.AddScoped<VeiculosService>();
builder.Services.AddScoped<ViagensService>();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<ManutencaoService>();
builder.Services.AddScoped<DashboardCombustivelService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestoreDeFrotas API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes("CHAVE_SECRETA_CENTRALIZADA_DO_PORTAL_INTERNO_2026")
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Swagger no Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

// Middleware Global
app.UseMiddleware<LoggingMiddleware>();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();


// 🔥 RESTAURADO — Endpoint de Teste para Geração de Token JWT
app.MapPost("/api/auth/teste-token",
    [Microsoft.AspNetCore.Authorization.AllowAnonymous] (string cargo) =>
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var chave = Encoding.ASCII.GetBytes("CHAVE_SECRETA_CENTRALIZADA_DO_PORTAL_INTERNO_2026");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "UtilizadorTeste"),
            new System.Security.Claims.Claim("roles", cargo)
        }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(chave),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Results.Ok(new { token = tokenString });
    });


// 🔥 RESTAURADO — Seed da Base de Dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    GestoreDeFrotas.Data.DbInitializer.Seed(context);
}

app.Run();
