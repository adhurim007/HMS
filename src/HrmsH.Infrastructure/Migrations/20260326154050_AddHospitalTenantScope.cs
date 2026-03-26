using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalTenantScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hospitals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospitals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_Code",
                table: "Hospitals",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Hospitals WHERE Code = 'DEFAULT-HOSP')
BEGIN
    INSERT INTO Hospitals (Name, Code, Address, CreatedAt, CreatedBy, IsDeleted)
    VALUES ('Default Hospital', 'DEFAULT-HOSP', 'Main', SYSUTCDATETIME(), 'migration', 0);
END
");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
DECLARE @defaultHospitalId INT = (SELECT TOP 1 Id FROM Hospitals WHERE Code = 'DEFAULT-HOSP');
UPDATE Facilities SET HospitalId = @defaultHospitalId WHERE HospitalId = 0;
");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_HospitalId",
                table: "Facilities",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Hospitals_HospitalId",
                table: "Facilities",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
DECLARE @defaultHospitalId2 INT = (SELECT TOP 1 Id FROM Hospitals WHERE Code = 'DEFAULT-HOSP');
UPDATE U
SET U.HospitalId = @defaultHospitalId2
FROM AspNetUsers U
WHERE U.HospitalId IS NULL
  AND EXISTS (
      SELECT 1
      FROM AspNetUserRoles UR
      INNER JOIN AspNetRoles R ON R.Id = UR.RoleId
      WHERE UR.UserId = U.Id
        AND R.Name <> 'SuperAdmin'
  );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Hospitals_HospitalId",
                table: "Facilities");

            migrationBuilder.DropTable(
                name: "Hospitals");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_HospitalId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "AspNetUsers");
        }
    }
}
