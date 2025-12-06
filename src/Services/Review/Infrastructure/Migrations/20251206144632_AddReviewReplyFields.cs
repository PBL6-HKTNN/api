using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Review.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewReplyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "repliedAt",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "repliedBy",
                table: "Reviews",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reply",
                table: "Reviews",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "repliedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "repliedBy",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "reply",
                table: "Reviews");
        }
    }
}
