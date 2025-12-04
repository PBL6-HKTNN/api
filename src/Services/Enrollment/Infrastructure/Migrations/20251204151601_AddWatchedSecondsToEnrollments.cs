using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Enrollment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchedSecondsToEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "watchedSeconds",
                table: "Enrollments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "watchedSeconds",
                table: "Enrollments");
        }
    }
}
