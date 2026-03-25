using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeRadiologyOrderNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyAppointments_DiagnosticOrders_DiagnosticOrderId",
                table: "RadiologyAppointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiagnosticOrders",
                table: "DiagnosticOrders");

            migrationBuilder.RenameTable(
                name: "DiagnosticOrders",
                newName: "RadiologyOrders");

            migrationBuilder.RenameIndex(
                name: "IX_DiagnosticOrders_DiagnosticTestId",
                table: "RadiologyOrders",
                newName: "IX_RadiologyOrders_DiagnosticTestId");

            migrationBuilder.RenameColumn(
                name: "DiagnosticOrderId",
                table: "RadiologyAppointments",
                newName: "RadiologyOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyAppointments_DiagnosticOrderId",
                table: "RadiologyAppointments",
                newName: "IX_RadiologyAppointments_RadiologyOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RadiologyOrders",
                table: "RadiologyOrders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyAppointments_RadiologyOrders_RadiologyOrderId",
                table: "RadiologyAppointments",
                column: "RadiologyOrderId",
                principalTable: "RadiologyOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyAppointments_RadiologyOrders_RadiologyOrderId",
                table: "RadiologyAppointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RadiologyOrders",
                table: "RadiologyOrders");

            migrationBuilder.RenameColumn(
                name: "RadiologyOrderId",
                table: "RadiologyAppointments",
                newName: "DiagnosticOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyAppointments_RadiologyOrderId",
                table: "RadiologyAppointments",
                newName: "IX_RadiologyAppointments_DiagnosticOrderId");

            migrationBuilder.RenameTable(
                name: "RadiologyOrders",
                newName: "DiagnosticOrders");

            migrationBuilder.RenameIndex(
                name: "IX_RadiologyOrders_DiagnosticTestId",
                table: "DiagnosticOrders",
                newName: "IX_DiagnosticOrders_DiagnosticTestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiagnosticOrders",
                table: "DiagnosticOrders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyAppointments_DiagnosticOrders_DiagnosticOrderId",
                table: "RadiologyAppointments",
                column: "DiagnosticOrderId",
                principalTable: "DiagnosticOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
