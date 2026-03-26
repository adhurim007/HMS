using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiFacilitySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SupplierReference",
                table: "PharmacyPurchaseInvoices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "PharmacyPurchaseInvoices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "PharmacyPurchaseInvoices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "PharmacyPurchaseInvoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "LaboratoryOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "InstallmentPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "InstallmentItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffFacilityAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffMemberId = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffFacilityAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffFacilityAssignments_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
UPDATE a
SET a.FacilityId = d.FacilityId
FROM Appointments a
INNER JOIN Departments d ON d.Id = a.DepartmentId
WHERE a.FacilityId IS NULL AND a.DepartmentId IS NOT NULL;");

            migrationBuilder.Sql(@"
UPDATE v
SET v.FacilityId = d.FacilityId
FROM Visits v
INNER JOIN StaffMembers s ON s.Id = v.DoctorId
INNER JOIN Departments d ON d.Id = s.DepartmentId
WHERE v.FacilityId IS NULL AND v.DoctorId IS NOT NULL AND s.DepartmentId IS NOT NULL;");

            migrationBuilder.Sql(@"
UPDATE i
SET i.FacilityId = lo.FacilityId
FROM Invoices i
INNER JOIN InvoiceItems ii ON ii.InvoiceId = i.Id
INNER JOIN LaboratoryOrderItems loi ON loi.Id = ii.LaboratoryOrderItemId
INNER JOIN LaboratoryOrders lo ON lo.Id = loi.LaboratoryOrderId
WHERE i.FacilityId IS NULL AND lo.FacilityId IS NOT NULL;");

            migrationBuilder.Sql(@"
UPDATE p SET p.FacilityId = i.FacilityId
FROM Payments p
INNER JOIN Invoices i ON i.Id = p.InvoiceId
WHERE p.FacilityId IS NULL;");

            migrationBuilder.Sql(@"
UPDATE ip SET ip.FacilityId = i.FacilityId
FROM InstallmentPlans ip
INNER JOIN Invoices i ON i.Id = ip.InvoiceId
WHERE ip.FacilityId IS NULL;");

            migrationBuilder.Sql(@"
UPDATE ii SET ii.FacilityId = ip.FacilityId
FROM InstallmentItems ii
INNER JOIN InstallmentPlans ip ON ip.Id = ii.InstallmentPlanId
WHERE ii.FacilityId IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_FacilityId",
                table: "Visits",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_FacilityId",
                table: "StockMovements",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyPurchaseInvoices_FacilityId",
                table: "PharmacyPurchaseInvoices",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyPurchaseInvoices_InvoiceNumber",
                table: "PharmacyPurchaseInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FacilityId",
                table: "Payments",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryOrders_FacilityId",
                table: "LaboratoryOrders",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FacilityId",
                table: "Invoices",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentPlans_FacilityId",
                table: "InstallmentPlans",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentItems_FacilityId",
                table: "InstallmentItems",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FacilityId",
                table: "Appointments",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffFacilityAssignments_StaffMemberId_FacilityId",
                table: "StaffFacilityAssignments",
                columns: new[] { "StaffMemberId", "FacilityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffFacilityAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Visits_FacilityId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_FacilityId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyPurchaseInvoices_FacilityId",
                table: "PharmacyPurchaseInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyPurchaseInvoices_InvoiceNumber",
                table: "PharmacyPurchaseInvoices");

            migrationBuilder.DropIndex(
                name: "IX_Payments_FacilityId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryOrders_FacilityId",
                table: "LaboratoryOrders");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_FacilityId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentPlans_FacilityId",
                table: "InstallmentPlans");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentItems_FacilityId",
                table: "InstallmentItems");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_FacilityId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "PharmacyPurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "LaboratoryOrders");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "InstallmentPlans");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "InstallmentItems");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierReference",
                table: "PharmacyPurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "PharmacyPurchaseInvoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "PharmacyPurchaseInvoices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
