using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrmsH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFacilityScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "AspNetUsers");
        }
    }
}
