using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewsSources_NewsSourceMappingFieldID",
                table: "NewsSources");

            migrationBuilder.CreateIndex(
                name: "IX_NewsSources_NewsSourceMappingFieldID",
                table: "NewsSources",
                column: "NewsSourceMappingFieldID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewsSources_NewsSourceMappingFieldID",
                table: "NewsSources");

            migrationBuilder.CreateIndex(
                name: "IX_NewsSources_NewsSourceMappingFieldID",
                table: "NewsSources",
                column: "NewsSourceMappingFieldID",
                unique: true);
        }
    }
}
