using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadBay.DataAccess.Migrations
{
    public partial class ChangedPostalCodeToPostCode_StateToCounty_ListPriceToRRP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListPrice",
                table: "Products",
                newName: "RRP");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "OrderHeaders",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "OrderHeaders",
                newName: "County");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "AspNetUsers",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "AspNetUsers",
                newName: "County");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RRP",
                table: "Products",
                newName: "ListPrice");

            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "OrderHeaders",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "County",
                table: "OrderHeaders",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "AspNetUsers",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "County",
                table: "AspNetUsers",
                newName: "PostalCode");
        }
    }
}
