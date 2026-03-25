using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLaboratoryItemsToInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BilledAt",
                table: "LaboratoryOrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBilled",
                table: "LaboratoryOrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LaboratoryOrderItemId",
                table: "InvoiceItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_LaboratoryOrderItemId",
                table: "InvoiceItems",
                column: "LaboratoryOrderItemId",
                unique: true,
                filter: "[LaboratoryOrderItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_LaboratoryOrderItems_LaboratoryOrderItemId",
                table: "InvoiceItems",
                column: "LaboratoryOrderItemId",
                principalTable: "LaboratoryOrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_LaboratoryOrderItems_LaboratoryOrderItemId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_LaboratoryOrderItemId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "BilledAt",
                table: "LaboratoryOrderItems");

            migrationBuilder.DropColumn(
                name: "IsBilled",
                table: "LaboratoryOrderItems");

            migrationBuilder.DropColumn(
                name: "LaboratoryOrderItemId",
                table: "InvoiceItems");
        }
    }
}
