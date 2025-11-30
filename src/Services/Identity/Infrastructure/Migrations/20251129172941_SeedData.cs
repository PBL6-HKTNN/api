using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Codemy.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    permissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    actionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    permissionName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    googleId = table.Column<string>(type: "text", nullable: false),
                    passwordHash = table.Column<string>(type: "text", nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    profilePicture = table.Column<string>(type: "text", nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    emailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    emailVerificationToken = table.Column<string>(type: "text", nullable: true),
                    resetPasswordToken = table.Column<string>(type: "text", nullable: true),
                    resetPasswordTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    totalCourses = table.Column<int>(type: "integer", nullable: true),
                    rating = table.Column<decimal>(type: "numeric", nullable: true),
                    totalLoginFailures = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Actions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), "CATEGORY_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new category", false, "CATEGORY_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444442"), "CATEGORY_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read category", false, "CATEGORY_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444443"), "CATEGORY_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update category information", false, "CATEGORY_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "CATEGORY_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete category", false, "CATEGORY_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444445"), "COURSE_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new course", false, "COURSE_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444446"), "COURSE_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read course", false, "COURSE_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444447"), "COURSE_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update course information", false, "COURSE_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444448"), "COURSE_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete course", false, "COURSE_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444449"), "LESSON_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new lesson", false, "LESSON_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444450"), "LESSON_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read lesson", false, "LESSON_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444451"), "LESSON_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update lesson information", false, "LESSON_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444452"), "LESSON_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete lesson", false, "LESSON_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444453"), "MODULE_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new module", false, "MODULE_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444454"), "MODULE_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read module", false, "MODULE_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444455"), "MODULE_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update module information", false, "MODULE_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444456"), "MODULE_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete module", false, "MODULE_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444457"), "QUIZ_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new quiz", false, "QUIZ_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444458"), "QUIZ_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read quiz", false, "QUIZ_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444459"), "QUIZ_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update quiz information", false, "QUIZ_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444460"), "QUIZ_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete quiz", false, "QUIZ_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444461"), "QUIZ_SUBMIT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Submit quiz", false, "QUIZ_SUBMIT", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444462"), "ENROLLMENT_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new enrollment", false, "ENROLLMENT_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444463"), "ENROLLMENT_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read enrollment", false, "ENROLLMENT_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444464"), "ENROLLMENT_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update enrollment information", false, "ENROLLMENT_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444465"), "ENROLLMENT_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete enrollment", false, "ENROLLMENT_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444466"), "WISHLIST_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new wishlist", false, "WISHLIST_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444467"), "WISHLIST_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read wishlist", false, "WISHLIST_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444468"), "WISHLIST_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update wishlist information", false, "WISHLIST_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444469"), "WISHLIST_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete wishlist", false, "WISHLIST_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444470"), "USER_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update user information", false, "USER_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444471"), "PAYMENT_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new payment", false, "PAYMENT_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444472"), "PAYMENT_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read payment", false, "PAYMENT_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444473"), "PAYMENT_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update payment information", false, "PAYMENT_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444474"), "PAYMENT_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete payment", false, "PAYMENT_DELETE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444475"), "REVIEW_CREATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Create a new review", false, "REVIEW_CREATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444476"), "REVIEW_READ", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Read review", false, "REVIEW_READ", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444477"), "REVIEW_UPDATE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Update review information", false, "REVIEW_UPDATE", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444478"), "REVIEW_DELETE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Delete review", false, "REVIEW_DELETE", null, null }
                });

            migrationBuilder.InsertData(
                table: "PermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "actionId", "permissionId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444504"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444441"), new Guid("44444444-4444-4444-4444-444444444494") },
                    { new Guid("44444444-4444-4444-4444-444444444505"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444442"), new Guid("44444444-4444-4444-4444-444444444494") },
                    { new Guid("44444444-4444-4444-4444-444444444506"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444443"), new Guid("44444444-4444-4444-4444-444444444494") },
                    { new Guid("44444444-4444-4444-4444-444444444507"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444444"), new Guid("44444444-4444-4444-4444-444444444494") },
                    { new Guid("44444444-4444-4444-4444-444444444508"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444445"), new Guid("44444444-4444-4444-4444-444444444495") },
                    { new Guid("44444444-4444-4444-4444-444444444509"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444446"), new Guid("44444444-4444-4444-4444-444444444495") },
                    { new Guid("44444444-4444-4444-4444-444444444510"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444447"), new Guid("44444444-4444-4444-4444-444444444495") },
                    { new Guid("44444444-4444-4444-4444-444444444511"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444448"), new Guid("44444444-4444-4444-4444-444444444495") },
                    { new Guid("44444444-4444-4444-4444-444444444512"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444449"), new Guid("44444444-4444-4444-4444-444444444496") },
                    { new Guid("44444444-4444-4444-4444-444444444513"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444450"), new Guid("44444444-4444-4444-4444-444444444496") },
                    { new Guid("44444444-4444-4444-4444-444444444514"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444451"), new Guid("44444444-4444-4444-4444-444444444496") },
                    { new Guid("44444444-4444-4444-4444-444444444515"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444452"), new Guid("44444444-4444-4444-4444-444444444496") },
                    { new Guid("44444444-4444-4444-4444-444444444516"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444453"), new Guid("44444444-4444-4444-4444-444444444497") },
                    { new Guid("44444444-4444-4444-4444-444444444517"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444454"), new Guid("44444444-4444-4444-4444-444444444497") },
                    { new Guid("44444444-4444-4444-4444-444444444518"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444455"), new Guid("44444444-4444-4444-4444-444444444497") },
                    { new Guid("44444444-4444-4444-4444-444444444519"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444456"), new Guid("44444444-4444-4444-4444-444444444497") },
                    { new Guid("44444444-4444-4444-4444-444444444520"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444457"), new Guid("44444444-4444-4444-4444-444444444498") },
                    { new Guid("44444444-4444-4444-4444-444444444521"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444458"), new Guid("44444444-4444-4444-4444-444444444498") },
                    { new Guid("44444444-4444-4444-4444-444444444522"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444459"), new Guid("44444444-4444-4444-4444-444444444498") },
                    { new Guid("44444444-4444-4444-4444-444444444523"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444460"), new Guid("44444444-4444-4444-4444-444444444498") },
                    { new Guid("44444444-4444-4444-4444-444444444524"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444461"), new Guid("44444444-4444-4444-4444-444444444498") },
                    { new Guid("44444444-4444-4444-4444-444444444525"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444462"), new Guid("44444444-4444-4444-4444-444444444499") },
                    { new Guid("44444444-4444-4444-4444-444444444526"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444463"), new Guid("44444444-4444-4444-4444-444444444499") },
                    { new Guid("44444444-4444-4444-4444-444444444527"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444464"), new Guid("44444444-4444-4444-4444-444444444499") },
                    { new Guid("44444444-4444-4444-4444-444444444528"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444465"), new Guid("44444444-4444-4444-4444-444444444499") },
                    { new Guid("44444444-4444-4444-4444-444444444529"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444466"), new Guid("44444444-4444-4444-4444-444444444500") },
                    { new Guid("44444444-4444-4444-4444-444444444530"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444467"), new Guid("44444444-4444-4444-4444-444444444500") },
                    { new Guid("44444444-4444-4444-4444-444444444531"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444468"), new Guid("44444444-4444-4444-4444-444444444500") },
                    { new Guid("44444444-4444-4444-4444-444444444532"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444469"), new Guid("44444444-4444-4444-4444-444444444500") },
                    { new Guid("44444444-4444-4444-4444-444444444533"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444470"), new Guid("44444444-4444-4444-4444-444444444501") },
                    { new Guid("44444444-4444-4444-4444-444444444534"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444471"), new Guid("44444444-4444-4444-4444-444444444502") },
                    { new Guid("44444444-4444-4444-4444-444444444535"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444472"), new Guid("44444444-4444-4444-4444-444444444502") },
                    { new Guid("44444444-4444-4444-4444-444444444536"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444473"), new Guid("44444444-4444-4444-4444-444444444502") },
                    { new Guid("44444444-4444-4444-4444-444444444537"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444474"), new Guid("44444444-4444-4444-4444-444444444502") },
                    { new Guid("44444444-4444-4444-4444-444444444538"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444475"), new Guid("44444444-4444-4444-4444-444444444503") },
                    { new Guid("44444444-4444-4444-4444-444444444539"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444476"), new Guid("44444444-4444-4444-4444-444444444503") },
                    { new Guid("44444444-4444-4444-4444-444444444540"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444477"), new Guid("44444444-4444-4444-4444-444444444503") },
                    { new Guid("44444444-4444-4444-4444-444444444541"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444478"), new Guid("44444444-4444-4444-4444-444444444503") },
                    { new Guid("44444444-4444-4444-4444-444444444542"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444442"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444543"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444446"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444544"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444450"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444545"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444454"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444546"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444458"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444547"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444461"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444548"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444462"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444549"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444463"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444550"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444464"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444551"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444466"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444552"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444467"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444553"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444468"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444554"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444469"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444555"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444470"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444556"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444471"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444557"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444472"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444558"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444473"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444559"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444475"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444560"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444476"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444561"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444477"), new Guid("44444444-4444-4444-4444-444444444542") },
                    { new Guid("44444444-4444-4444-4444-444444444562"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444442"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444563"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444457"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444564"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444458"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444565"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444459"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444566"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444460"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444567"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444463"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444568"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444470"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444569"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444471"), new Guid("44444444-4444-4444-4444-444444444562") },
                    { new Guid("44444444-4444-4444-4444-444444444570"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, new Guid("44444444-4444-4444-4444-444444444476"), new Guid("44444444-4444-4444-4444-444444444562") }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "permissionName" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444494"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Category Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444495"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Course Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444496"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Lesson Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444497"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Module Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444498"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Quiz Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444499"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Enrollment Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444500"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Wishlist Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444501"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "User Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444502"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Payment Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444503"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Review Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444542"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Student Permission" },
                    { new Guid("44444444-4444-4444-4444-444444444562"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, "Other permission of Instructor" }
                });

            migrationBuilder.InsertData(
                table: "UserPermissionGroups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444132"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444494"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444136"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444495"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444140"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444496"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444144"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444497"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444148"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444498"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444153"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444499"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444157"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444500"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444161"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444501"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444162"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444502"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444166"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444503"), 0, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444170"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444495"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444174"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444496"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444178"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444497"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444182"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444562"), 2, null, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444192"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, new Guid("44444444-4444-4444-4444-444444444542"), 3, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "UpdatedAt", "UpdatedBy", "bio", "email", "emailVerificationToken", "emailVerified", "googleId", "name", "passwordHash", "profilePicture", "rating", "resetPasswordToken", "resetPasswordTokenExpiry", "role", "status", "totalCourses", "totalLoginFailures" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444479"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "huukhoa04Ins@gmail.com", null, true, "", "Nguyễn Hữu Khoa", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 2, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444480"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "custina0987123Ins@gmail.com", null, true, "", "Lê Xuân Hòa", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 2, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444481"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "minhnguyetdn2004Ins@gmail.com", null, true, "", "Huỳnh Thúy Minh Nguyệt", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 2, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444482"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "daolehanhnguyenIns@gmail.com", null, true, "", "Đào Lê Hạnh Nguyên", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 2, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444483"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "nauts010203Ins@gmail.com", null, true, "", "Võ Văn Tuấn", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 2, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444484"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "huukhoa04Mod@gmail.com", null, true, "", "Nguyễn Hữu Khoa", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 1, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444485"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "custina0987123Mod@gmail.com", null, true, "", "Lê Xuân Hòa", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 1, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444486"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "minhnguyetdn2004Mod@gmail.com", null, true, "", "Huỳnh Thúy Minh Nguyệt", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 1, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444487"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "daolehanhnguyenMod@gmail.com", null, true, "", "Đào Lê Hạnh Nguyên", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 1, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444488"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "nauts010203Mod@gmail.com", null, true, "", "Võ Văn Tuấn", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 1, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444489"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "huukhoa04@gmail.com", null, true, "", "Nguyễn Hữu Khoa", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 0, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444490"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "custina0987123@gmail.com", null, true, "", "Lê Xuân Hòa", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 0, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444491"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "minhnguyetdn2004@gmail.com", null, true, "", "Huỳnh Thúy Minh Nguyệt", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 0, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444492"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "daolehanhnguyen@gmail.com", null, true, "", "Đào Lê Hạnh Nguyên", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 0, 0, null, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444493"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, null, null, null, "nauts010203@gmail.com", null, true, "", "Võ Văn Tuấn", "AQAAAAIAAYagAAAAEFJUguByi3SGjbuZdLh/cwh5ZpMnUb8rWajofPA/7z7tmadNRFcOtR4Np5b6wUPjxw==", null, null, null, null, 0, 0, null, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "PermissionGroups");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "UserPermissionGroups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
