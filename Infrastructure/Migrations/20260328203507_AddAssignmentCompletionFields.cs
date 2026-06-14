using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentCompletionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerSignature",
                table: "StaffAssignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerSignedAt",
                table: "StaffAssignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSignedName",
                table: "StaffAssignments",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KmDriven",
                table: "StaffAssignments",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSignature",
                table: "StaffAssignments");

            migrationBuilder.DropColumn(
                name: "CustomerSignedAt",
                table: "StaffAssignments");

            migrationBuilder.DropColumn(
                name: "CustomerSignedName",
                table: "StaffAssignments");

            migrationBuilder.DropColumn(
                name: "KmDriven",
                table: "StaffAssignments");
        }
    }
}
