using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarNotificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataConclusao",
                table: "RegistosManutencao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPrevista",
                table: "RegistosManutencao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Data",
                table: "Notificacoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Notificacoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VeiculoId1",
                table: "Notificacoes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "DocumentosVeiculos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VeiculoId1",
                table: "DocumentosVeiculos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistoricoVeiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeiculoId = table.Column<int>(type: "int", nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoVeiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoVeiculos_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_VeiculoId1",
                table: "Notificacoes",
                column: "VeiculoId1");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosVeiculos_VeiculoId1",
                table: "DocumentosVeiculos",
                column: "VeiculoId1");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoVeiculos_VeiculoId",
                table: "HistoricoVeiculos",
                column: "VeiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentosVeiculos_Veiculos_VeiculoId1",
                table: "DocumentosVeiculos",
                column: "VeiculoId1",
                principalTable: "Veiculos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacoes_Veiculos_VeiculoId1",
                table: "Notificacoes",
                column: "VeiculoId1",
                principalTable: "Veiculos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentosVeiculos_Veiculos_VeiculoId1",
                table: "DocumentosVeiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificacoes_Veiculos_VeiculoId1",
                table: "Notificacoes");

            migrationBuilder.DropTable(
                name: "HistoricoVeiculos");

            migrationBuilder.DropIndex(
                name: "IX_Notificacoes_VeiculoId1",
                table: "Notificacoes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentosVeiculos_VeiculoId1",
                table: "DocumentosVeiculos");

            migrationBuilder.DropColumn(
                name: "DataConclusao",
                table: "RegistosManutencao");

            migrationBuilder.DropColumn(
                name: "DataPrevista",
                table: "RegistosManutencao");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "VeiculoId1",
                table: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "DocumentosVeiculos");

            migrationBuilder.DropColumn(
                name: "VeiculoId1",
                table: "DocumentosVeiculos");
        }
    }
}
