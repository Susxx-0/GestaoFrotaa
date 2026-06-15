using FluentValidation;
using GestoreDeFrotas.Data;
using GestoreDeFrotas.Models;
using GestoreDeFrotas.Services;
using GestoreDeFrotas.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog só na consola (sem ficheiro)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GestaoFrotasDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

builder.Services.AddScoped<IValidator<Veiculo>, VeiculoValidator>();
builder.Services.AddScoped<VeiculosService>();
builder.Services.AddScoped<ManutencaoService>();
builder.Services.AddScoped<AbastecimentosService>();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<NotificacoesService>();
builder.Services.AddScoped<IValidator<Abastecimento>, AbastecimentoValidator>();
builder.Services.AddScoped<IValidator<RegistoManutencao>, RegistoManutencaoValidator>();
builder.Services.AddScoped<IValidator<DocumentoUploadDto>, DocumentoUploadValidator>();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
builder.Services.AddScoped<ViagensService>();

builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<DashboardCombustivelService>();
builder.Services.AddScoped<DashboardUtilizacaoService>();
builder.Services.AddScoped<DashboardAlertasService>();
builder.Services.AddScoped<DashboardManutencaoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    AppDbContext.SeedData(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Gestão de Frotas API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();

// Middleware de logging/auditoria (BD)
app.UseMiddleware<LoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    GestoreDeFrotas.Data.DbInitializer.Seed(context);
}

app.MapControllers();

app.Run();
