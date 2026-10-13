using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cad.Server.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class CasosSos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "casos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    origen = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ciudadano_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_solicitud = table.Column<Guid>(type: "uuid", nullable: false),
                    latitud = table.Column<double>(type: "double precision", nullable: true),
                    longitud = table.Column<double>(type: "double precision", nullable: true),
                    precision_metros = table.Column<double>(type: "double precision", nullable: true),
                    ubicacion_obtenida_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_casos", x => x.id);
                    table.CheckConstraint("ck_casos_latitud", "latitud IS NULL OR latitud BETWEEN -90 AND 90");
                    table.CheckConstraint("ck_casos_longitud", "longitud IS NULL OR longitud BETWEEN -180 AND 180");
                    table.CheckConstraint("ck_casos_precision", "precision_metros IS NULL OR precision_metros >= 0");
                    table.CheckConstraint("ck_casos_ubicacion_completa", "(latitud IS NULL) = (longitud IS NULL) AND (latitud IS NULL) = (precision_metros IS NULL) AND (latitud IS NULL) = (ubicacion_obtenida_en IS NULL)");
                    table.ForeignKey(
                        name: "fk_casos_asp_net_users_ciudadano_id",
                        column: x => x.ciudadano_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "eventos_caso",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    caso_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visibilidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    datos = table.Column<string>(type: "jsonb", nullable: true),
                    ocurrido_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_eventos_caso", x => x.id);
                    table.ForeignKey(
                        name: "fk_eventos_caso_casos_caso_id",
                        column: x => x.caso_id,
                        principalTable: "casos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_casos_estado",
                table: "casos",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "ux_casos_ciudadano_id_solicitud",
                table: "casos",
                columns: new[] { "ciudadano_id", "id_solicitud" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_eventos_caso_caso_id_ocurrido_en",
                table: "eventos_caso",
                columns: new[] { "caso_id", "ocurrido_en" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eventos_caso");

            migrationBuilder.DropTable(
                name: "casos");
        }
    }
}
