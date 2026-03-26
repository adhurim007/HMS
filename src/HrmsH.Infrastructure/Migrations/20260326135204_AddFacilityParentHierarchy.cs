using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacilityParentHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Facilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_ParentId",
                table: "Facilities",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Facilities_ParentId",
                table: "Facilities",
                column: "ParentId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Facilities_ParentId",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_ParentId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Facilities");
        }
    }
}
