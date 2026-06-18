using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoHttp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rota = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Utilizador = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    CategoriaUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstaAtivo = table.Column<bool>(type: "bit", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KmAtual = table.Column<int>(type: "int", nullable: false),
                    UltimaManutencaoKm = table.Column<int>(type: "int", nullable: true),
                    UltimaManutencaoData = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProximaManutencaoKm = table.Column<int>(type: "int", nullable: true),
                    ProximaManutencaoData = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataProximaIpo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataInspecao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataSeguro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Abastecimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    Litros = table.Column<double>(type: "float", nullable: false),
                    PrecoPorLitro = table.Column<double>(type: "float(18)", precision: 18, scale: 3, nullable: false),
                    CustoTotal = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    KmAtual = table.Column<int>(type: "int", nullable: false),
                    KmPorLitro = table.Column<double>(type: "float", nullable: true),
                    ConsumoMedio = table.Column<double>(type: "float", nullable: true),
                    Combustivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Posto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abastecimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Abastecimentos_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosVeiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeFicheiroOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaminhoFicheiro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosVeiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosVeiculos_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    VeiculoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacoes_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RegistosManutencao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Custo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RealizadoPor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Km = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistosManutencao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistosManutencao_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Viagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    CondutorPrincipalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondutorSecundarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KmIniciais = table.Column<int>(type: "int", nullable: false),
                    KmFinais = table.Column<int>(type: "int", nullable: true),
                    DataLimitePrevista = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstaAtiva = table.Column<bool>(type: "bit", nullable: false),
                    PediuProrrogacao = table.Column<bool>(type: "bit", nullable: false),
                    ObservacoesEntrega = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Viagens_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abastecimentos_VeiculoId",
                table: "Abastecimentos",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosVeiculos_VeiculoId",
                table: "DocumentosVeiculos",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_VeiculoId",
                table: "Notificacoes",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistosManutencao_VeiculoId",
                table: "RegistosManutencao",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Viagens_VeiculoId",
                table: "Viagens",
                column: "VeiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abastecimentos");

            migrationBuilder.DropTable(
                name: "DocumentosVeiculos");

            migrationBuilder.DropTable(
                name: "LogsSistema");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropTable(
                name: "RegistosManutencao");

            migrationBuilder.DropTable(
                name: "Viagens");

            migrationBuilder.DropTable(
                name: "Veiculos");
        }
    }
}
