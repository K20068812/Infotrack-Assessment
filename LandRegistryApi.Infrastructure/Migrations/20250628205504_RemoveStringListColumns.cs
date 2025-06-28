using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LandRegistryApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStringListColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalResults",
                table: "SearchResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalResults",
                table: "SearchResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
