using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartFieldsAndMSL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Parts",
                newName: "StockQty");

            migrationBuilder.AddColumn<string>(
                name: "AlternateLocation",
                table: "Parts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoxNo",
                table: "Parts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MSL",
                table: "Parts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StorageLocation",
                table: "Parts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateLocation",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "BoxNo",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "MSL",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "StorageLocation",
                table: "Parts");

            migrationBuilder.RenameColumn(
                name: "StockQty",
                table: "Parts",
                newName: "Stock");
        }
    }
}
