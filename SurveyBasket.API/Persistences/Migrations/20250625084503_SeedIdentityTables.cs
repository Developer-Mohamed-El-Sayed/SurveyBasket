using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.API.Persistences.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "037C8F57-42E8-4BF1-8B8D-AD87D6EF118C", "B3A09AB8-213A-401C-BED7-C38E45BD2280", true, false, "Member", "MEMBER" },
                    { "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F", "64F1C648-83C4-4293-BB0F-27A79DE4AE64", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0FD4A426-BDD8-4EC4-AC89-1E9C23169E87", 0, "EC38839F-5B2C-4D3C-9CCF-894AFDA0BD4F", "admin@survey-basket.com", true, "Survey", "Basket", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEAvzIEJWyfsX9Q54e7JayOvm+4fOT42xITgQFu3QnrF+pxDaFglRqm2W0tS+zauCwQ==", "+201002308834", true, "BF24BD4AB8A243C4A8C7699FAFEBF7A0", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 2, "permissions", "polls:add", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 3, "permissions", "polls:update", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 4, "permissions", "polls:delete", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 5, "permissions", "questions:read", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 6, "permissions", "questions:add", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 7, "permissions", "questions:update", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 8, "permissions", "users:read", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 9, "permissions", "users:add", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 10, "permissions", "users:update", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 11, "permissions", "roles:read", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 12, "permissions", "roles:add", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 13, "permissions", "roles:update", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" },
                    { 14, "permissions", "results:read", "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F", "0FD4A426-BDD8-4EC4-AC89-1E9C23169E87" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "037C8F57-42E8-4BF1-8B8D-AD87D6EF118C");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F", "0FD4A426-BDD8-4EC4-AC89-1E9C23169E87" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B62C9CA0-474D-42A1-ADF1-A672EE2AEA1F");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0FD4A426-BDD8-4EC4-AC89-1E9C23169E87");
        }
    }
}
