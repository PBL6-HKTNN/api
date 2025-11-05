using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnTableUserAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "marksObtained",
                table: "UserAnswers");

            migrationBuilder.AlterColumn<Guid>(
                name: "answerId",
                table: "UserAnswers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "answerText",
                table: "UserAnswers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "answerText",
                table: "UserAnswers");

            migrationBuilder.AlterColumn<Guid>(
                name: "answerId",
                table: "UserAnswers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "marksObtained",
                table: "UserAnswers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
