using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoadmapAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("14444444-4444-4444-4444-444444444582"), "ROADMAP_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create roadmap", false, "ROADMAP_CREATE", null, null },
                    { new Guid("14444444-4444-4444-4444-444444444583"), "ROADMAP_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read roadmap", false, "ROADMAP_READ", null, null },
                    { new Guid("14444444-4444-4444-4444-444444444584"), "ROADMAP_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update roadmap", false, "ROADMAP_UPDATE", null, null },
                    { new Guid("14444444-4444-4444-4444-444444444585"), "ROADMAP_JOIN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Join roadmap", false, "ROADMAP_JOIN", null, null },
                    { new Guid("14444444-4444-4444-4444-444444444586"), "ROADMAP_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete roadmap", false, "ROADMAP_DELETE", null, null }
                });

            migrationBuilder.InsertData(
                table: "PermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "actionId", "permissionId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-5444-444444444580"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("14444444-4444-4444-4444-444444444582"), new Guid("44444444-4444-4444-4444-444444445567") },
                    { new Guid("44444444-4444-4444-5444-444444444581"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("14444444-4444-4444-4444-444444444583"), new Guid("44444444-4444-4444-4444-444444445567") },
                    { new Guid("44444444-4444-4444-5444-444444444582"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("14444444-4444-4444-4444-444444444584"), new Guid("44444444-4444-4444-4444-444444445567") },
                    { new Guid("44444444-4444-4444-5444-444444444583"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("14444444-4444-4444-4444-444444444585"), new Guid("44444444-4444-4444-4444-444444445567") },
                    { new Guid("44444444-4444-4444-5444-444444444584"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("14444444-4444-4444-4444-444444444586"), new Guid("44444444-4444-4444-4444-444444445567") }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "permissionName" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444445567"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Roadmap Permission" });

            migrationBuilder.InsertData(
                table: "UserPermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444186"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444445567"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444195"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444445567"), 3, null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("14444444-4444-4444-4444-444444444582"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("14444444-4444-4444-4444-444444444583"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("14444444-4444-4444-4444-444444444584"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("14444444-4444-4444-4444-444444444585"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("14444444-4444-4444-4444-444444444586"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444580"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444581"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444582"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444583"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-5444-444444444584"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444445567"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444186"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444195"));
        }
    }
}
