using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CriticalThinkingGame.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGameTextTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTextTranslations");

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "GameTexts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GameTexts_LanguageId",
                table: "GameTexts",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameTexts_Languages_LanguageId",
                table: "GameTexts",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameTexts_Languages_LanguageId",
                table: "GameTexts");

            migrationBuilder.DropIndex(
                name: "IX_GameTexts_LanguageId",
                table: "GameTexts");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "GameTexts");

            migrationBuilder.CreateTable(
                name: "GameTextTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameTextId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTextTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTextTranslations_GameTexts_GameTextId",
                        column: x => x.GameTextId,
                        principalTable: "GameTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameTextTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameTextTranslations_GameTextId_LanguageId",
                table: "GameTextTranslations",
                columns: new[] { "GameTextId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameTextTranslations_LanguageId",
                table: "GameTextTranslations",
                column: "LanguageId");
        }
    }
}
