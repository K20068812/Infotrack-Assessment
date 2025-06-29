using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistryApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsForAllColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SearchResults_TargetUrl_SearchDate",
                table: "SearchResults");

            migrationBuilder.AlterColumn<string>(
                name: "SearchEngine",
                table: "SearchResults",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchEngine",
                table: "SearchResults",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_SearchResults_TargetUrl_SearchDate",
                table: "SearchResults",
                columns: new[] { "TargetUrl", "SearchDate" });
        }
    }
}
