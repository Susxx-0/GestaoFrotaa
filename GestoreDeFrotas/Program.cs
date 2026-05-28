using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using GestaoDeFrotas.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Base de Dados REAL no SQL Server (Substituiu o In-Memory)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GestaoFrotasDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddScoped<GestaoDeFrotas.Services.VeiculosService>();
builder.Services.AddScoped<GestaoDeFrotas.Services.ManutencaoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger com suporte para colar o Token (Cadeado)
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer SEU_TOKEN_AQUI",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var chaveSecretaGlobal = "CHAVE_SECRETA_CENTRALIZADA_DO_PORTAL_INTERNO_2026";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(chaveSecretaGlobal)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = "roles"
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var identity = context.Principal?.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var consolaRole = identity.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                if (!string.IsNullOrEmpty(consolaRole) && !identity.HasClaim(c => c.Type == "roles"))
                {
                    identity.AddClaim(new Claim("roles", consolaRole));
                }
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// 3. Executa as Migrações e Alimenta a API com os 32 veículos fakes
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Cria a Base de Dados real caso ela não exista no SQL Server
    context.Database.Migrate();

    AppDbContext.SeedData(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint temporário para gerar um Token de teste diretamente no Swagger
app.MapPost("/api/auth/teste-token", [Microsoft.AspNetCore.Authorization.AllowAnonymous] (string cargo) =>
{
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var chave = Encoding.ASCII.GetBytes(chaveSecretaGlobal);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new System.Security.Claims.ClaimsIdentity(new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "EstagiarioFrotas"),
            new System.Security.Claims.Claim("roles", cargo)
        }),
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new { token = tokenHandler.WriteToken(token) });
});

app.MapControllers();

app.Run();