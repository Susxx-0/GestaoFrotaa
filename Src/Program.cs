using FluentValidation;
using FluentValidation.AspNetCore;
using GestoreDeFrotas.Data;
using GestoreDeFrotas.Middleware;
using GestoreDeFrotas.Services;
using GestoreDeFrotas.Services.Abastecimentos;
using GestoreDeFrotas.Services.Dashboard;
using GestoreDeFrotas.Services.Documentos;
using GestoreDeFrotas.Services.Manutencao;
using GestoreDeFrotas.Services.Notificacoes;
using GestoreDeFrotas.Services.OCR;
using GestoreDeFrotas.Services.Relatorios;
using GestoreDeFrotas.Services.Sistema;
using GestoreDeFrotas.Services.Veiculos;
using GestoreDeFrotas.Services.Viagens;
using GestoreDeFrotas.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegistoManutencaoValidator>();

// ----------------------------
//  DATABASE
// ----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------
//  SERVICES
// ----------------------------
builder.Services.AddScoped<VehicleService>();
builder.Services.AddSingleton<OcrService>();
builder.Services.AddSingleton<CartaParserService>();
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<AbastecimentoService>();
builder.Services.AddScoped<ViagensService>();
builder.Services.AddScoped<DocumentosService>();
builder.Services.AddScoped<DocumentosPesquisaService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<PDFService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<AtribuicaoVeiculoService>();
builder.Services.AddScoped<DashboardKpiService>();
builder.Services.AddScoped<DashboardGraficosService>();







// ----------------------------
//  AUTENTICAÇÃO JWT
// ----------------------------
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

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
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


// ----------------------------
//  CONTROLLERS
// ----------------------------
builder.Services.AddControllers();

// ----------------------------
//  SWAGGER + JWT
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gestore de Frotas", Version = "v1" });

    // Adicionar suporte a JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insere o token JWT assim: Bearer {teu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// ----------------------------
//  MIDDLEWARE
// ----------------------------
app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestore de Frotas v2");
    c.RoutePrefix = "swagger";
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
