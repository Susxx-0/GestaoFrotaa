using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntervaloManutencaoKm",
                table: "Veiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KmAtual",
                table: "Veiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaManutencaoData",
                table: "Veiculos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UltimaManutencaoKm",
                table: "Veiculos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Km",
                table: "RegistosManutencao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntervaloManutencaoKm",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "KmAtual",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "UltimaManutencaoData",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "UltimaManutencaoKm",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "Km",
                table: "RegistosManutencao");
        }
    }
}
