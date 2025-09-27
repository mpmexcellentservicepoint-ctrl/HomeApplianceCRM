using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductWarrantyAndLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DealerId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyExpiryDate",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarrantyType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerId",
                table: "Products",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DealerId",
                table: "Products",
                column: "DealerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Customers_CustomerId",
                table: "Products",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Dealers_DealerId",
                table: "Products",
                column: "DealerId",
                principalTable: "Dealers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Customers_CustomerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Dealers_DealerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DealerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DealerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WarrantyExpiryDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WarrantyType",
                table: "Products");
        }
    }
}
