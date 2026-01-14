using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TFG_Cultivos.Migrations
{
    /// <inheritdoc />
    public partial class AddPropuestasCultivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropuestasCultivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    RecintoId = table.Column<int>(type: "integer", nullable: false),
                    AnioCampania = table.Column<int>(type: "integer", nullable: false),
                    CultivoPropuesto = table.Column<string>(type: "text", nullable: false),
                    Justificacion = table.Column<string>(type: "text", nullable: false),
                    EsBorrador = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasCultivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestasCultivo_Recintos_RecintoId",
                        column: x => x.RecintoId,
                        principalTable: "Recintos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasCultivo_RecintoId",
                table: "PropuestasCultivo",
                column: "RecintoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropuestasCultivo");
        }
    }
}
