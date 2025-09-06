using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryStoreManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
     name: "Password",
     table: "Customers",
     type: "nvarchar(20)",
     maxLength: 20,
     nullable: false,
     defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "User");

            //migrationBuilder.CreateTable(
            //    name: "ProductCategories",
            //    columns: table => new
            //    {
            //        CategoryID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductCategories", x => x.CategoryID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Orders",
            //    columns: table => new
            //    {
            //        OrderID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerID = table.Column<int>(type: "int", nullable: false),
            //        OrderDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        TotalAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Orders", x => x.OrderID);
            //        table.ForeignKey(
            //            name: "FK_Orders_Customers",
            //            column: x => x.CustomerID,
            //            principalTable: "Customers",
            //            principalColumn: "CustomerID");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Products",
            //    columns: table => new
            //    {
            //        ProductID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        CategoryID = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
            //        UsedStock = table.Column<int>(type: "int", nullable: false),
            //        RemainingStock = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Products", x => x.ProductID);
            //        table.ForeignKey(
            //            name: "FK_Products_ProductCategories",
            //            column: x => x.CategoryID,
            //            principalTable: "ProductCategories",
            //            principalColumn: "CategoryID");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OrderDetails",
            //    columns: table => new
            //    {
            //        OrderDetailsID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        OrderID = table.Column<int>(type: "int", nullable: false),
            //        ProductID = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailsID);
            //        table.ForeignKey(
            //            name: "FK_OrderDetails_Orders",
            //            column: x => x.OrderID,
            //            principalTable: "Orders",
            //            principalColumn: "OrderID");
            //        table.ForeignKey(
            //            name: "FK_OrderDetails_Products",
            //            column: x => x.ProductID,
            //            principalTable: "Products",
            //            principalColumn: "ProductID");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderDetails_OrderID",
            //    table: "OrderDetails",
            //    column: "OrderID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderDetails_ProductID",
            //    table: "OrderDetails",
            //    column: "ProductID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Orders_CustomerID",
            //    table: "Orders",
            //    column: "CustomerID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Products_CategoryID",
            //    table: "Products",
            //    column: "CategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "OrderDetails");

            //migrationBuilder.DropTable(
            //    name: "Orders");

            //migrationBuilder.DropTable(
            //    name: "Products");

            //migrationBuilder.DropTable(
            //    name: "Customers");

            migrationBuilder.DropColumn(
    name: "Password",
    table: "Customers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Customers");

            //migrationBuilder.DropTable(
            //    name: "ProductCategories");
        }
    }
}
