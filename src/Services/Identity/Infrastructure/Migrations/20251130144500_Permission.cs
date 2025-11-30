using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Permission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserPermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444167"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444572"), 0, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444167"));
        }
    }
}
