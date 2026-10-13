using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cad.Server.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class MedicionPresentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mediciones_presentacion",
                columns: table => new
                {
                    caso_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aceptado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    acuse_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    milisegundos = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mediciones_presentacion", x => x.caso_id);
                    table.ForeignKey(
                        name: "fk_mediciones_presentacion_casos_caso_id",
                        column: x => x.caso_id,
                        principalTable: "casos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mediciones_presentacion");
        }
    }
}
