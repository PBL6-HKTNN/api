using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class totalEnrollmentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "totalEnrollments",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "totalEnrollments",
                table: "Courses");
        }
    }
}
