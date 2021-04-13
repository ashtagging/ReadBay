using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadBay.DataAccess.Migrations
{
    public partial class Change_state_and_postalCode_for_company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "Companies",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Companies",
                newName: "County");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "Companies",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "County",
                table: "Companies",
                newName: "PostalCode");
        }
    }
}
