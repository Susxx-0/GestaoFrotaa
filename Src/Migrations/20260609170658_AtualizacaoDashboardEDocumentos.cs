using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoDashboardEDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Notificacoes",
                newName: "Grau");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Notificacoes",
                newName: "DestinatarioId");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Notificacoes",
                newName: "DataCriacao");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "IntervaloManutencaoKm",
                table: "Veiculos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaUsuario",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CondutorHabitualId",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataProximaIpo",
                table: "Veiculos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstaAtivo",
                table: "Veiculos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TecnicoResponsavelId",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VeiculoId",
                table: "Notificacoes",
                type: "int",
                nullable: true);

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
                    ObservacoesEntrega = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstaAtiva = table.Column<bool>(type: "bit", nullable: false),
                    DataLimitePrevista = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PediuProrrogacao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viagens", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Viagens");

            migrationBuilder.DropColumn(
                name: "CategoriaUsuario",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "CondutorHabitualId",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "DataProximaIpo",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "EstaAtivo",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "TecnicoResponsavelId",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "VeiculoId",
                table: "Notificacoes");

            migrationBuilder.RenameColumn(
                name: "Grau",
                table: "Notificacoes",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "DestinatarioId",
                table: "Notificacoes",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "Notificacoes",
                newName: "Data");

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "Veiculos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "IntervaloManutencaoKm",
                table: "Veiculos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
