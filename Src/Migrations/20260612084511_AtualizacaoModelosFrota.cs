using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestoreDeFrotas.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoModelosFrota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KmFinais",
                table: "Viagens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KmIniciais",
                table: "Viagens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TecnicoLevantamentoId",
                table: "Viagens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TecnicoRecebimentoId",
                table: "Viagens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KmFinais",
                table: "Viagens");

            migrationBuilder.DropColumn(
                name: "KmIniciais",
                table: "Viagens");

            migrationBuilder.DropColumn(
                name: "TecnicoLevantamentoId",
                table: "Viagens");

            migrationBuilder.DropColumn(
                name: "TecnicoRecebimentoId",
                table: "Viagens");
        }
    }
}
