using FluentValidation;
using FluentValidation.AspNetCore;
using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services;
using GestoreDeFrotas.Validators;
using GestoreDeFrotas.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTO DE LOGS
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

// 2. INFRAESTRUTURA EF CORE COM SQL SERVER
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=GestaoFrotasDB;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// 3. REFATORAÇÃO INTELIGENTE: REGISTO AUTOMÁTICO DE TODOS OS VALIDADORES NUMA SÓ LINHA
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<VeiculoValidator>();

// 4. DEPENDÊNCIAS DE CORE E DASHBOARD UNIFICADAS
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<NotificacoesService>();
builder.Services.AddScoped<VeiculosService>();
builder.Services.AddScoped<ManutencaoService>();
builder.Services.AddScoped<AbastecimentosService>();
builder.Services.AddScoped<ViagensService>();

builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<DashboardCombustivelService>();
builder.Services.AddScoped<DashboardUtilizacaoService>();
builder.Services.AddScoped<DashboardAlertasService>();
builder.Services.AddScoped<DashboardManutencaoService>();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 5. CONFIGURAÇÃO DE SEGURANÇA NO SWAGGER
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token desta forma: Bearer SEU_TOKEN_AQUI",
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
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// 6. AUTENTICAÇÃO JWT TRATADA COM CORREÇÃO CRÍTICA DE ASSINATURA
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
        ValidateIssuerSigningKey = true,
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

// 7. PIPELINE HTTP E INTERFACES GRAFICAS
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Gestão de Frotas API").WithTheme(ScalarTheme.DeepSpace).WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<LoggingMiddleware>(); // <--- AGORA SEU MIDDLEWARE REAL JÁ CONSEGUE ENTRAR NO PIPELINE DE FORMA SEGURA
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

// 8. GERADOR DE TOKEN DE TESTE
app.MapPost("/api/auth/teste-token", [Microsoft.AspNetCore.Authorization.AllowAnonymous] (string cargo) =>
{
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var chave = Encoding.ASCII.GetBytes(chaveSecretaGlobal);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "EstagiarioFrotas"), new Claim("roles", cargo) }),
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
    };
    return Results.Ok(new { token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)) });
});

// 9. RESOLUÇÃO COMPLETA DO ERRO DE DOUBLE-SEEDING E MIGRATIONS CONCORRENTES
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

app.MapControllers();
app.Run();