using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewsSources_NewsSourceTokenID",
                table: "NewsSources");

            migrationBuilder.CreateIndex(
                name: "IX_NewsSources_NewsSourceTokenID",
                table: "NewsSources",
                column: "NewsSourceTokenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewsSources_NewsSourceTokenID",
                table: "NewsSources");

            migrationBuilder.CreateIndex(
                name: "IX_NewsSources_NewsSourceTokenID",
                table: "NewsSources",
                column: "NewsSourceTokenID",
                unique: true);
        }
    }
}
