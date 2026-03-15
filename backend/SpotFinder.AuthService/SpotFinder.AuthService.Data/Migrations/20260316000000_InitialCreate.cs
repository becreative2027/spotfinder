using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotFinder.AuthService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "auth");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "local"),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "user"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_tokens",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_favorites",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    venue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorites", x => new { x.user_id, x.venue_id });
                    table.ForeignKey(
                        name: "FK_user_favorites_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_visits",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    venue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visited_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_visits", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_visits_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "auth",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_refresh_tokens_user_id",
                schema: "auth",
                table: "user_refresh_tokens",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "user_visits", schema: "auth");
            migrationBuilder.DropTable(name: "user_favorites", schema: "auth");
            migrationBuilder.DropTable(name: "user_refresh_tokens", schema: "auth");
            migrationBuilder.DropTable(name: "users", schema: "auth");
        }
    }
}
