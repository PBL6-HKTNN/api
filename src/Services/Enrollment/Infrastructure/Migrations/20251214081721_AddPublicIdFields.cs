using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Enrollment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "certificatePublicId",
                table: "Enrollments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "certificatePublicId",
                table: "Enrollments");
        }
    }
}
