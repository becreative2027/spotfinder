using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SpotFinder.VenueService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "venue");

            migrationBuilder.CreateTable(
                name: "concept_tags",
                schema: "venue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameTr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_concept_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                schema: "venue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "İstanbul")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "venues",
                schema: "venue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParkingStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Lat = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    Lng = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_venues_districts_DistrictId",
                        column: x => x.DistrictId,
                        principalSchema: "venue",
                        principalTable: "districts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "venue_concepts",
                schema: "venue",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConceptTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venue_concepts", x => new { x.VenueId, x.ConceptTagId });
                    table.ForeignKey(
                        name: "FK_venue_concepts_concept_tags_ConceptTagId",
                        column: x => x.ConceptTagId,
                        principalSchema: "venue",
                        principalTable: "concept_tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_venue_concepts_venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "venue",
                        principalTable: "venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "venue_photos",
                schema: "venue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsMenuPhoto = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venue_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_venue_photos_venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "venue",
                        principalTable: "venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "venue",
                table: "concept_tags",
                columns: new[] { "Id", "CreatedAt", "IsActive", "IsSystem", "NameEn", "NameTr" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Birthday", "Doğum Günü" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Romantic", "Romantik" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Scenic View", "Manzaralı" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Outdoor", "Açık Hava" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Breakfast", "Kahvaltı" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Live Music", "Canlı Müzik" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Family Friendly", "Aile Dostu" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Business Lunch", "İş Yemeği" },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Vegan / Vegetarian", "Vegan / Vejetaryen" },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Waterfront", "Deniz Kenarı" },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Coffee & Dessert", "Kahve & Tatlı" },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Nightlife", "Gece Hayatı" },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Brunch", "Brunch" },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Fast Food", "Fast Food" },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Fine Dining", "Fine Dining" },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Historic Venue", "Tarihî Mekân" },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Pet Friendly", "Köpek Dostu" },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Tea Garden", "Çay Bahçesi" },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Sports Event", "Spor Etkinliği" },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Art & Culture", "Sanat & Kültür" },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Bosphorus View", "Boğaz Manzarası" },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Hookah", "Nargile" },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Playground", "Oyun Alanı" },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Rooftop", "Rooftop" },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Work Friendly", "Çalışma Dostu" }
                });

            migrationBuilder.InsertData(
                schema: "venue",
                table: "districts",
                columns: new[] { "Id", "City", "Name" },
                values: new object[,]
                {
                    { 1, "İstanbul", "Adalar" },
                    { 2, "İstanbul", "Arnavutköy" },
                    { 3, "İstanbul", "Ataşehir" },
                    { 4, "İstanbul", "Avcılar" },
                    { 5, "İstanbul", "Bağcılar" },
                    { 6, "İstanbul", "Bahçelievler" },
                    { 7, "İstanbul", "Bakırköy" },
                    { 8, "İstanbul", "Başakşehir" },
                    { 9, "İstanbul", "Bayrampaşa" },
                    { 10, "İstanbul", "Beşiktaş" },
                    { 11, "İstanbul", "Beykoz" },
                    { 12, "İstanbul", "Beylikdüzü" },
                    { 13, "İstanbul", "Beyoğlu" },
                    { 14, "İstanbul", "Büyükçekmece" },
                    { 15, "İstanbul", "Çatalca" },
                    { 16, "İstanbul", "Çekmeköy" },
                    { 17, "İstanbul", "Esenler" },
                    { 18, "İstanbul", "Esenyurt" },
                    { 19, "İstanbul", "Eyüpsultan" },
                    { 20, "İstanbul", "Fatih" },
                    { 21, "İstanbul", "Gaziosmanpaşa" },
                    { 22, "İstanbul", "Güngören" },
                    { 23, "İstanbul", "Kadıköy" },
                    { 24, "İstanbul", "Kağıthane" },
                    { 25, "İstanbul", "Kartal" },
                    { 26, "İstanbul", "Küçükçekmece" },
                    { 27, "İstanbul", "Maltepe" },
                    { 28, "İstanbul", "Pendik" },
                    { 29, "İstanbul", "Sancaktepe" },
                    { 30, "İstanbul", "Sarıyer" },
                    { 31, "İstanbul", "Silivri" },
                    { 32, "İstanbul", "Sultanbeyli" },
                    { 33, "İstanbul", "Sultangazi" },
                    { 34, "İstanbul", "Şile" },
                    { 35, "İstanbul", "Şişli" },
                    { 36, "İstanbul", "Tuzla" },
                    { 37, "İstanbul", "Ümraniye" },
                    { 38, "İstanbul", "Üsküdar" },
                    { 39, "İstanbul", "Zeytinburnu" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_venue_concepts_ConceptTagId",
                schema: "venue",
                table: "venue_concepts",
                column: "ConceptTagId");

            migrationBuilder.CreateIndex(
                name: "IX_venue_photos_VenueId",
                schema: "venue",
                table: "venue_photos",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_venues_DistrictId",
                schema: "venue",
                table: "venues",
                column: "DistrictId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "venue_concepts",
                schema: "venue");

            migrationBuilder.DropTable(
                name: "venue_photos",
                schema: "venue");

            migrationBuilder.DropTable(
                name: "concept_tags",
                schema: "venue");

            migrationBuilder.DropTable(
                name: "venues",
                schema: "venue");

            migrationBuilder.DropTable(
                name: "districts",
                schema: "venue");
        }
    }
}
