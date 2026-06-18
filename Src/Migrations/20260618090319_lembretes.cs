using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    /// <inheritdoc />
    public partial class lembretes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntervaloManutencaoKm",
                table: "Veiculos",
                newName: "ProximaManutencaoKm");

            migrationBuilder.AlterColumn<int>(
                name: "UltimaManutencaoKm",
                table: "Veiculos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Cor",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Veiculos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInspecao",
                table: "Veiculos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataSeguro",
                table: "Veiculos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximaManutencaoData",
                table: "Veiculos",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "DataInspecao",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "DataSeguro",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "ProximaManutencaoData",
                table: "Veiculos");

            migrationBuilder.RenameColumn(
                name: "ProximaManutencaoKm",
                table: "Veiculos",
                newName: "IntervaloManutencaoKm");

            migrationBuilder.AlterColumn<int>(
                name: "UltimaManutencaoKm",
                table: "Veiculos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cor",
                table: "Veiculos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
