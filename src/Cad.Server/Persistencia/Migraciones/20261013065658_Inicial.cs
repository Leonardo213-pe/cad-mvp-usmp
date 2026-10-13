using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cad.Server.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    creado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rol_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rol_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_rol_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estados_sereno",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operativo = table.Column<bool>(type: "boolean", nullable: false),
                    latitud = table.Column<double>(type: "double precision", nullable: true),
                    longitud = table.Column<double>(type: "double precision", nullable: true),
                    precision_metros = table.Column<double>(type: "double precision", nullable: true),
                    posicion_obtenida_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    actualizado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estados_sereno", x => x.usuario_id);
                    table.CheckConstraint("ck_estados_sereno_latitud", "latitud IS NULL OR latitud BETWEEN -90 AND 90");
                    table.CheckConstraint("ck_estados_sereno_longitud", "longitud IS NULL OR longitud BETWEEN -180 AND 180");
                    table.CheckConstraint("ck_estados_sereno_posicion_completa", "(latitud IS NULL) = (longitud IS NULL) AND (latitud IS NULL) = (precision_metros IS NULL) AND (latitud IS NULL) = (posicion_obtenida_en IS NULL)");
                    table.ForeignKey(
                        name: "fk_estados_sereno_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "perfiles_ciudadano",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_completo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    registrado_en = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_perfiles_ciudadano", x => x.usuario_id);
                    table.ForeignKey(
                        name: "fk_perfiles_ciudadano_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuario_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuario_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_usuario_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_usuario_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuario_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_usuario_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { new Guid("6d1e0b7a-4c1f-4f3e-9a51-000000000001"), "6d1e0b7a-4c1f-4f3e-9a51-000000000001", "Ciudadano", "CIUDADANO" },
                    { new Guid("6d1e0b7a-4c1f-4f3e-9a51-000000000002"), "6d1e0b7a-4c1f-4f3e-9a51-000000000002", "Operador", "OPERADOR" },
                    { new Guid("6d1e0b7a-4c1f-4f3e-9a51-000000000003"), "6d1e0b7a-4c1f-4f3e-9a51-000000000003", "Sereno", "SERENO" },
                    { new Guid("6d1e0b7a-4c1f-4f3e-9a51-000000000004"), "6d1e0b7a-4c1f-4f3e-9a51-000000000004", "Administrador", "ADMINISTRADOR" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_rol_claims_role_id",
                table: "rol_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuario_claims_user_id",
                table: "usuario_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuario_logins_user_id",
                table: "usuario_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuario_roles_role_id",
                table: "usuario_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuarios",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_email_normalizado",
                table: "usuarios",
                column: "normalized_email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estados_sereno");

            migrationBuilder.DropTable(
                name: "perfiles_ciudadano");

            migrationBuilder.DropTable(
                name: "rol_claims");

            migrationBuilder.DropTable(
                name: "usuario_claims");

            migrationBuilder.DropTable(
                name: "usuario_logins");

            migrationBuilder.DropTable(
                name: "usuario_roles");

            migrationBuilder.DropTable(
                name: "usuario_tokens");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
