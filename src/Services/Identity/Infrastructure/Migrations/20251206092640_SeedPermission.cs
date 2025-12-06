using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("44444444-4444-4444-4444-644444444583"), "PERMISSION_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read my permission", false, "MY_PERMISSION_READ", null, null });

            migrationBuilder.InsertData(
                table: "PermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "actionId", "permissionId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-5444-444444444561"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-644444444583"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-5444-444444444571"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-644444444583"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-5444-444444444579"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-644444444583"), new Guid("44444444-4444-4444-4444-444444444563") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-644444444583"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444561"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444571"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444579"));
        }
    }
}
