using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionItemBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBilled",
                table: "PrescriptionItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PrescriptionItemId",
                table: "InvoiceItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_PrescriptionItemId",
                table: "InvoiceItems",
                column: "PrescriptionItemId",
                unique: true,
                filter: "[PrescriptionItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_PrescriptionItems_PrescriptionItemId",
                table: "InvoiceItems",
                column: "PrescriptionItemId",
                principalTable: "PrescriptionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_PrescriptionItems_PrescriptionItemId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_PrescriptionItemId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "IsBilled",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "PrescriptionItemId",
                table: "InvoiceItems");
        }
    }
}
