using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorie",
                columns: table => new
                {
                    CategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorie", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    KeywordID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeywordText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.KeywordID);
                });

            migrationBuilder.CreateTable(
                name: "NewsSources",
                columns: table => new
                {
                    NewsSourceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BaseURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsSources", x => x.NewsSourceID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    NewsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NewsSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.NewsID);
                    table.ForeignKey(
                        name: "FK_News_NewsSources_NewsSourceId",
                        column: x => x.NewsSourceId,
                        principalTable: "NewsSources",
                        principalColumn: "NewsSourceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsSourceTokens",
                columns: table => new
                {
                    NewsSourceTokenID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NewsSourceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsSourceTokens", x => x.NewsSourceTokenID);
                    table.ForeignKey(
                        name: "FK_NewsSourceTokens_NewsSources_NewsSourceID",
                        column: x => x.NewsSourceID,
                        principalTable: "NewsSources",
                        principalColumn: "NewsSourceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserKeywords",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeywordID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeywords", x => new { x.UserID, x.KeywordID });
                    table.ForeignKey(
                        name: "FK_UserKeywords_Keywords_KeywordID",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserKeywords_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsCategories",
                columns: table => new
                {
                    NewsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategories", x => new { x.NewsID, x.CategoryID });
                    table.ForeignKey(
                        name: "FK_NewsCategories_Categorie_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categorie",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsCategories_News_NewsID",
                        column: x => x.NewsID,
                        principalTable: "News",
                        principalColumn: "NewsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsKeywords",
                columns: table => new
                {
                    NewsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeywordID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsKeywords", x => new { x.NewsID, x.KeywordID });
                    table.ForeignKey(
                        name: "FK_NewsKeywords_Keywords_KeywordID",
                        column: x => x.KeywordID,
                        principalTable: "Keywords",
                        principalColumn: "KeywordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsKeywords_News_NewsID",
                        column: x => x.NewsID,
                        principalTable: "News",
                        principalColumn: "NewsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedNews",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedNews", x => new { x.UserId, x.NewsId });
                    table.ForeignKey(
                        name: "FK_SavedNews_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "NewsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedNews_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsSourceId",
                table: "News",
                column: "NewsSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsCategories_CategoryID",
                table: "NewsCategories",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsKeywords_KeywordID",
                table: "NewsKeywords",
                column: "KeywordID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsSourceTokens_NewsSourceID",
                table: "NewsSourceTokens",
                column: "NewsSourceID");

            migrationBuilder.CreateIndex(
                name: "IX_SavedNews_NewsId",
                table: "SavedNews",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKeywords_KeywordID",
                table: "UserKeywords",
                column: "KeywordID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsCategories");

            migrationBuilder.DropTable(
                name: "NewsKeywords");

            migrationBuilder.DropTable(
                name: "NewsSourceTokens");

            migrationBuilder.DropTable(
                name: "SavedNews");

            migrationBuilder.DropTable(
                name: "UserKeywords");

            migrationBuilder.DropTable(
                name: "Categorie");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "NewsSources");
        }
    }
}
