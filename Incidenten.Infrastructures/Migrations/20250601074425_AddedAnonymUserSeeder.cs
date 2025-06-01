using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Incidenten.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class AddedAnonymUserSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "Password", "Role", "SendNotifications", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 6, 1, 7, 44, 24, 795, DateTimeKind.Utc).AddTicks(9904), "", "", "", 0, true, new DateTime(2025, 6, 1, 7, 44, 24, 795, DateTimeKind.Utc).AddTicks(9906) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));
        }
    }
}
