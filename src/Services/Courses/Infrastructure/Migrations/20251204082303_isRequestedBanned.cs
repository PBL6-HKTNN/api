using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class isRequestedBanned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isRequestedBanned",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isRequestedBanned",
                table: "Courses");
        }
    }
}
