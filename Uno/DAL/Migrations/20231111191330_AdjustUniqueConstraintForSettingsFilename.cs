using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustUniqueConstraintForSettingsFilename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameSettings_FileName",
                table: "GameSettings");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_FileName_SettingName",
                table: "GameSettings",
                columns: new[] { "FileName", "SettingName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameSettings_FileName_SettingName",
                table: "GameSettings");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_FileName",
                table: "GameSettings",
                column: "FileName",
                unique: true);
        }
    }
}
