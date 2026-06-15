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
