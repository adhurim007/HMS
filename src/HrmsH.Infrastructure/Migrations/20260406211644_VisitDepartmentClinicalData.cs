using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VisitDepartmentClinicalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visits_PatientId",
                table: "Visits");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Visits",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChiefComplaint",
                table: "Visits",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicalDataJson",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisitFormTemplate",
                table: "Visits",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "GENERAL");

            migrationBuilder.AddColumn<string>(
                name: "ParentGuardianName",
                table: "Patients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PediatricGjtl",
                table: "Patients",
                type: "decimal(9,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PediatricMtl",
                table: "Patients",
                type: "decimal(9,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PediatricPkl",
                table: "Patients",
                type: "decimal(9,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PriorAbortion",
                table: "Patients",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PriorLiveBirth",
                table: "Patients",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId_VisitDate",
                table: "Visits",
                columns: new[] { "DoctorId", "VisitDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PatientId_VisitDate",
                table: "Visits",
                columns: new[] { "PatientId", "VisitDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_VisitFormTemplate",
                table: "Visits",
                column: "VisitFormTemplate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visits_DoctorId_VisitDate",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_PatientId_VisitDate",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_VisitFormTemplate",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ClinicalDataJson",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "VisitFormTemplate",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ParentGuardianName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PediatricGjtl",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PediatricMtl",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PediatricPkl",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PriorAbortion",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PriorLiveBirth",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Visits",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChiefComplaint",
                table: "Visits",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PatientId",
                table: "Visits",
                column: "PatientId");
        }
    }
}
