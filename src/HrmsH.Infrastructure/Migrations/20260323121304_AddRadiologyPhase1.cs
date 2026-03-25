using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRadiologyPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ContrastRequired",
                table: "DiagnosticTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DiagnosticTests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "DiagnosticTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImagingCategoryId",
                table: "DiagnosticTests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PriorPreparationRequired",
                table: "DiagnosticTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ClinicalIndication",
                table: "DiagnosticOrders",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "DiagnosticOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ImagingCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagingCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiagnosticOrderId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Equipment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RadiologistId = table.Column<int>(type: "int", nullable: true),
                    TechnicianId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyAppointments_DiagnosticOrders_DiagnosticOrderId",
                        column: x => x.DiagnosticOrderId,
                        principalTable: "DiagnosticOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyStudies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadiologyAppointmentId = table.Column<int>(type: "int", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EquipmentUsed = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyStudies_RadiologyAppointments_RadiologyAppointmentId",
                        column: x => x.RadiologyAppointmentId,
                        principalTable: "RadiologyAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadiologyStudyId = table.Column<int>(type: "int", nullable: false),
                    FilePathOrUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ThumbnailPath = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyAttachments_RadiologyStudies_RadiologyStudyId",
                        column: x => x.RadiologyStudyId,
                        principalTable: "RadiologyStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadiologyStudyId = table.Column<int>(type: "int", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Conclusion = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    ReportedById = table.Column<int>(type: "int", nullable: false),
                    ValidatedById = table.Column<int>(type: "int", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyReports_RadiologyStudies_RadiologyStudyId",
                        column: x => x.RadiologyStudyId,
                        principalTable: "RadiologyStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticTests_ImagingCategoryId",
                table: "DiagnosticTests",
                column: "ImagingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagingCategories_Code",
                table: "ImagingCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyAppointments_DiagnosticOrderId",
                table: "RadiologyAppointments",
                column: "DiagnosticOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyAttachments_RadiologyStudyId",
                table: "RadiologyAttachments",
                column: "RadiologyStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyReports_RadiologyStudyId",
                table: "RadiologyReports",
                column: "RadiologyStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudies_RadiologyAppointmentId",
                table: "RadiologyStudies",
                column: "RadiologyAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosticTests_ImagingCategories_ImagingCategoryId",
                table: "DiagnosticTests",
                column: "ImagingCategoryId",
                principalTable: "ImagingCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosticTests_ImagingCategories_ImagingCategoryId",
                table: "DiagnosticTests");

            migrationBuilder.DropTable(
                name: "ImagingCategories");

            migrationBuilder.DropTable(
                name: "RadiologyAttachments");

            migrationBuilder.DropTable(
                name: "RadiologyReports");

            migrationBuilder.DropTable(
                name: "RadiologyStudies");

            migrationBuilder.DropTable(
                name: "RadiologyAppointments");

            migrationBuilder.DropIndex(
                name: "IX_DiagnosticTests_ImagingCategoryId",
                table: "DiagnosticTests");

            migrationBuilder.DropColumn(
                name: "ContrastRequired",
                table: "DiagnosticTests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DiagnosticTests");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "DiagnosticTests");

            migrationBuilder.DropColumn(
                name: "ImagingCategoryId",
                table: "DiagnosticTests");

            migrationBuilder.DropColumn(
                name: "PriorPreparationRequired",
                table: "DiagnosticTests");

            migrationBuilder.DropColumn(
                name: "ClinicalIndication",
                table: "DiagnosticOrders");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "DiagnosticOrders");
        }
    }
}
