using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PermissionAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444579"), "PERMISSION_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new permission", false, "PERMISSION_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444580"), "PERMISSION_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read permission", false, "PERMISSION_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444581"), "PERMISSION_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update permission", false, "PERMISSION_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444582"), "PERMISSION_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete permission", false, "PERMISSION_DELETE", null, null }
                });

            migrationBuilder.InsertData(
                table: "PermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "actionId", "permissionId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444572"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444479"), new Guid("44444444-4444-4444-4444-444444444572") },
                    { new Guid("44444444-4444-4444-4444-444444444573"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444480"), new Guid("44444444-4444-4444-4444-444444444572") },
                    { new Guid("44444444-4444-4444-4444-444444444574"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444481"), new Guid("44444444-4444-4444-4444-444444444572") },
                    { new Guid("44444444-4444-4444-4444-444444444575"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444482"), new Guid("44444444-4444-4444-4444-444444444572") }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "permissionName" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444572"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Permission Management Permission" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444579"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444580"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444581"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444582"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444572"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444573"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444574"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444575"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444572"));
        }
    }
}
