using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistryApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchEngineColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchEngine",
                table: "SearchResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchEngine",
                table: "SearchResults");
        }
    }
}
