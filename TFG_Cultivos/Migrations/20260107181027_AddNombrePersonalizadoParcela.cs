using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFG_Cultivos.Migrations
{
    /// <inheritdoc />
    public partial class AddNombrePersonalizadoParcela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombrePersonalizado",
                table: "parcelas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombrePersonalizado",
                table: "parcelas");
        }
    }
}
