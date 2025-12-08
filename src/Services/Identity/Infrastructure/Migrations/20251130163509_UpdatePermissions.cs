using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserPermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444168"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444542"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444183"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444542"), 2, null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444168"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444183"));
        }
    }
}
