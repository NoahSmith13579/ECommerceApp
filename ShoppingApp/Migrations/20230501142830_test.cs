using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApp.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "OrderItem",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductID",
                table: "OrderItem",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Product_ProductID",
                table: "OrderItem",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Product_ProductID",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_ProductID",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderItem",
                newName: "ID");
        }
    }
}
