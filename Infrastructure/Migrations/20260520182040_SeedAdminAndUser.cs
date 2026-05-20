using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Role" },
                values: new object[,]
                {
                    { new Guid("a12b4e91-1a22-4b1c-bd6e-884d44e363f2"), "admin@example.com", "$2a$12$v7QEJTY67SZKKP0g94O/s.Q8A0HLLHC5HLbcMCGUGGeDUcx3/HeZa", "Admin" },
                    { new Guid("b74a3d90-7f89-4d2a-8b6c-773c33d252e1"), "user@example.com", "$2a$12$v7QEJTY67SZKKP0g94O/s.Q8A0HLLHC5HLbcMCGUGGeDUcx3/HeZa", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a12b4e91-1a22-4b1c-bd6e-884d44e363f2"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b74a3d90-7f89-4d2a-8b6c-773c33d252e1"));
        }
    }
}
