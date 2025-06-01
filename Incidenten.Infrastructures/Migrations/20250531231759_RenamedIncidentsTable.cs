using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Incidenten.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class RenamedIncidentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incident_users_ExecutorId",
                table: "Incident");

            migrationBuilder.DropForeignKey(
                name: "FK_Incident_users_ReporterId",
                table: "Incident");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Incident",
                table: "Incident");

            migrationBuilder.RenameTable(
                name: "Incident",
                newName: "incidents");

            migrationBuilder.RenameIndex(
                name: "IX_Incident_ReporterId",
                table: "incidents",
                newName: "IX_incidents_ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_Incident_ExecutorId",
                table: "incidents",
                newName: "IX_incidents_ExecutorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_incidents",
                table: "incidents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_incidents_users_ExecutorId",
                table: "incidents",
                column: "ExecutorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_incidents_users_ReporterId",
                table: "incidents",
                column: "ReporterId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_incidents_users_ExecutorId",
                table: "incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_incidents_users_ReporterId",
                table: "incidents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_incidents",
                table: "incidents");

            migrationBuilder.RenameTable(
                name: "incidents",
                newName: "Incident");

            migrationBuilder.RenameIndex(
                name: "IX_incidents_ReporterId",
                table: "Incident",
                newName: "IX_Incident_ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_incidents_ExecutorId",
                table: "Incident",
                newName: "IX_Incident_ExecutorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Incident",
                table: "Incident",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_users_ExecutorId",
                table: "Incident",
                column: "ExecutorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Incident_users_ReporterId",
                table: "Incident",
                column: "ReporterId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
