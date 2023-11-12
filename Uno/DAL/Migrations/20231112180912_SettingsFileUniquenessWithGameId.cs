using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SettingsFileUniquenessWithGameId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameSettings_FileName_SettingName",
                table: "GameSettings");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_FileName_SettingName_GameId",
                table: "GameSettings",
                columns: new[] { "FileName", "SettingName", "GameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameSettings_FileName_SettingName_GameId",
                table: "GameSettings");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_FileName_SettingName",
                table: "GameSettings",
                columns: new[] { "FileName", "SettingName" },
                unique: true);
        }
    }
}
