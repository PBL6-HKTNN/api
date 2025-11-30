using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444572"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444579"));

            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444573"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444580"));

            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444574"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444581"));

            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444575"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444582"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444572"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444479"));

            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444573"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444480"));

            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444574"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444481"));

            migrationBuilder.UpdateData(
                table: "PermissionGroups",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444575"),
                column: "actionId",
                value: new Guid("44444444-4444-4444-4444-444444444482"));
        }
    }
}
