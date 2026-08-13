using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientSignOffToStaffAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualHours",
                table: "StaffAssignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSignature",
                table: "StaffAssignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "KilometersDriven",
                table: "StaffAssignments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedOffAt",
                table: "StaffAssignments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "StaffAssignments");

            migrationBuilder.DropColumn(
                name: "ClientSignature",
                table: "StaffAssignments");

            migrationBuilder.DropColumn(
                name: "KilometersDriven",
                table: "StaffAssignments");

            migrationBuilder.DropColumn(
                name: "SignedOffAt",
                table: "StaffAssignments");
        }
    }
}
