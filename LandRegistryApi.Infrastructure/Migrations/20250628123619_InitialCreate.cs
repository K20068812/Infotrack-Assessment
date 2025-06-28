using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistryApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchQuery = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Positions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SearchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalResults = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchResults_TargetUrl_SearchDate",
                table: "SearchResults",
                columns: new[] { "TargetUrl", "SearchDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchResults");
        }
    }
}
