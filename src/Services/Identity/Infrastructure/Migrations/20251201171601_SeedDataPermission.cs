using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444533"));

            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444583"), "USER_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read user information", false, "USER_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444584"), "USER_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create user", false, "USER_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444585"), "USER_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete user", false, "USER_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444586"), "REQUEST_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update request information", false, "REQUEST_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444587"), "REQUEST_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read request information", false, "REQUEST_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444588"), "REQUEST_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create request", false, "REQUEST_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444589"), "REQUEST_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete request", false, "REQUEST_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444590"), "REQUEST_RESOLVE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Resolve request", false, "REQUEST_RESOLVE", null, null }
                });

            migrationBuilder.InsertData(
                table: "PermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "actionId", "permissionId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444601"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444470"), new Guid("44444444-4444-4444-4444-444444444501") },
                    { new Guid("44444444-4444-4444-4444-444444444602"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444583"), new Guid("44444444-4444-4444-4444-444444444501") },
                    { new Guid("44444444-4444-4444-4444-444444444603"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444584"), new Guid("44444444-4444-4444-4444-444444444501") },
                    { new Guid("44444444-4444-4444-4444-444444444604"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444585"), new Guid("44444444-4444-4444-4444-444444444501") },
                    { new Guid("44444444-4444-4444-4444-444444444605"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444583"), new Guid("44444444-4444-4444-4444-444444444580") },
                    { new Guid("44444444-4444-4444-4444-444444444606"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444584"), new Guid("44444444-4444-4444-4444-444444444577") },
                    { new Guid("44444444-4444-4444-4444-444444444607"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444585"), new Guid("44444444-4444-4444-4444-444444444577") },
                    { new Guid("44444444-4444-4444-4444-444444444608"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444586"), new Guid("44444444-4444-4444-4444-444444444576") },
                    { new Guid("44444444-4444-4444-4444-444444444609"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444587"), new Guid("44444444-4444-4444-4444-444444444576") },
                    { new Guid("44444444-4444-4444-4444-444444444610"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444588"), new Guid("44444444-4444-4444-4444-444444444576") },
                    { new Guid("44444444-4444-4444-4444-444444444611"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444589"), new Guid("44444444-4444-4444-4444-444444444578") },
                    { new Guid("44444444-4444-4444-4444-444444444612"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444590"), new Guid("44444444-4444-4444-4444-444444444579") },
                    { new Guid("44444444-4444-4444-4444-444444444613"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444587"), new Guid("44444444-4444-4444-4444-444444444579") }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "permissionName" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444576"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Request Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444577"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Admin User Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444578"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Admin Request Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444579"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Moderator Request Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444580"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "User Read Permission" }
                });

            migrationBuilder.InsertData(
                table: "UserPermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444169"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444580"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444171"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444577"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444172"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444578"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444184"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444580"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444185"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444576"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444193"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444580"), 3, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444194"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444576"), 3, null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444583"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444584"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444585"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444586"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444587"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444588"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444589"));

            migrationBuilder.DeleteData(
                table: "Actions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444590"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444601"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444602"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444603"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444604"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444605"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444606"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444607"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444608"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444609"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444610"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444611"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444612"));

            migrationBuilder.DeleteData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444613"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444576"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444577"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444578"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444579"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444580"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444169"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444171"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444172"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444184"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444185"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444193"));

            migrationBuilder.DeleteData(
                table: "UserPermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444194"));

            migrationBuilder.InsertData(
                table: "PermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "actionId", "permissionId" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444533"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444470"), new Guid("44444444-4444-4444-4444-444444444501") });
        }
    }
}
