using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TFG_Cultivos.Migrations
{
    /// <inheritdoc />
    public partial class InitialPAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parcelas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CodigoProvincia = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Municipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoAgregado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Zona = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Poligono = table.Column<int>(type: "integer", nullable: false),
                    ParcelaNumero = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parcelas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recintos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParcelaId = table.Column<int>(type: "integer", nullable: false),
                    IdRecinto = table.Column<int>(type: "integer", nullable: false),
                    UsoSigpac = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SuperficieSigpac = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recintos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recintos_parcelas_ParcelaId",
                        column: x => x.ParcelaId,
                        principalTable: "parcelas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatoAgronomico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecintoId = table.Column<int>(type: "integer", nullable: false),
                    AñoCampaña = table.Column<int>(type: "integer", nullable: false),
                    SuperficieCultivada = table.Column<decimal>(type: "numeric", nullable: false),
                    EspecieVariedad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EcoregimenPractica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SecanoRegadio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CultivoPrincipalSecundario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AireLibreProtegido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoAgronomico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatoAgronomico_Recintos_RecintoId",
                        column: x => x.RecintoId,
                        principalTable: "Recintos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatoAgronomico_RecintoId",
                table: "DatoAgronomico",
                column: "RecintoId");

            migrationBuilder.CreateIndex(
                name: "IX_Recintos_ParcelaId",
                table: "Recintos",
                column: "ParcelaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatoAgronomico");

            migrationBuilder.DropTable(
                name: "Recintos");

            migrationBuilder.DropTable(
                name: "parcelas");
        }
    }
}
