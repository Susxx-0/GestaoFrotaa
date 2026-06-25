using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    public partial class CriarTabelasCorretas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================
            // 1. Criar tabela Users
            // ============================
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Telefone = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    Role = table.Column<string>(nullable: false),
                    Ativo = table.Column<bool>(nullable: false),
                    CartaConducaoNumero = table.Column<string>(nullable: true),
                    CartaConducaoValidade = table.Column<DateTime>(nullable: true),
                    CartaConducaoFicheiro = table.Column<string>(nullable: true),
                    CartaConducaoCategoria = table.Column<string>(nullable: true),
                    DataCriacao = table.Column<DateTime>(nullable: false),
                    UltimoLogin = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // ============================
            // 2. Criar tabela AtribuicoesVeiculo
            // ============================
            migrationBuilder.CreateTable(
                name: "AtribuicoesVeiculo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    VeiculoId = table.Column<int>(nullable: false),
                    DataAtribuicao = table.Column<DateTime>(nullable: false),
                    DataDevolucao = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtribuicoesVeiculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtribuicoesVeiculo_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtribuicoesVeiculo_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtribuicoesVeiculo_UserId",
                table: "AtribuicoesVeiculo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AtribuicoesVeiculo_VeiculoId",
                table: "AtribuicoesVeiculo",
                column: "VeiculoId");

            // ============================
            // 3. FK Notificacoes → Users
            // ============================
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Notificacoes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_UserId",
                table: "Notificacoes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacoes_Users_UserId",
                table: "Notificacoes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacoes_Users_UserId",
                table: "Notificacoes");

            migrationBuilder.DropIndex(
                name: "IX_Notificacoes_UserId",
                table: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notificacoes");

            migrationBuilder.DropTable(
                name: "AtribuicoesVeiculo");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
