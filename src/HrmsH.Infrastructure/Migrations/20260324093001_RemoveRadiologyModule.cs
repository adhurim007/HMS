using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRadiologyModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "RadiologyOrders");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "ImagingCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagingCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiagnosticTestId = table.Column<int>(type: "int", nullable: false),
                    ClinicalIndication = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ResultNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResultValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyOrders_DiagnosticTests_DiagnosticTestId",
                        column: x => x.DiagnosticTestId,
                        principalTable: "DiagnosticTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadiologyOrderId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RadiologistId = table.Column<int>(type: "int", nullable: true),
                    Room = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyAppointments_RadiologyOrders_RadiologyOrderId",
                        column: x => x.RadiologyOrderId,
                        principalTable: "RadiologyOrders",
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentUsed = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePathOrUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailPath = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Conclusion = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportedById = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidatedById = table.Column<int>(type: "int", nullable: true)
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
                name: "IX_RadiologyAppointments_RadiologyOrderId",
                table: "RadiologyAppointments",
                column: "RadiologyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyAttachments_RadiologyStudyId",
                table: "RadiologyAttachments",
                column: "RadiologyStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_DiagnosticTestId",
                table: "RadiologyOrders",
                column: "DiagnosticTestId");

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
    }
}
