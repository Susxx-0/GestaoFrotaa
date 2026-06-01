using GestaoDeFrotas.Data;
using GestaoDeFrotas.Middleware;
using GestaoDeFrotas.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 🔹 SERILOG
// -----------------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// -----------------------------
// 🔹 BASE DE DADOS
// -----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GestaoFrotasDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

// -----------------------------
// 🔹 SERVICES
// -----------------------------
builder.Services.AddScoped<VeiculosService>();
builder.Services.AddScoped<ManutencaoService>();
builder.Services.AddScoped<AbastecimentosService>();
builder.Services.AddScoped<AuditoriaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -----------------------------
// 🔹 SWAGGER + TOKEN
// -----------------------------
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

// -----------------------------
// 🔹 AUTENTICAÇÃO JWT
// -----------------------------
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

// -----------------------------
// 🔹 MIGRAÇÕES AUTOMÁTICAS
// -----------------------------    
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    AppDbContext.SeedData(context);
}

// -----------------------------
// 🔹 SWAGGER
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// -----------------------------
// 🔹 MIDDLEWARE DE LOGGING
// -----------------------------
app.UseMiddleware<LoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// -----------------------------
// 🔹 ENDPOINT PARA TESTAR TOKEN
// -----------------------------
app.MapPost("/api/auth/teste-token", [Microsoft.AspNetCore.Authorization.AllowAnonymous] (string cargo) =>
{
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var chave = Encoding.ASCII.GetBytes(chaveSecretaGlobal);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "EstagiarioFrotas"),
            new Claim("roles", cargo)
        }),
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new { token = tokenHandler.WriteToken(token) });
});

app.MapControllers();

app.Run();
