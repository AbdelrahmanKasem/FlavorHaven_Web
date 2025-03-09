using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMSProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class FinalDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "Tables",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Menus",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MenuItemId",
                table: "Components",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MenuId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsLocation",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupOrder_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupOrder_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupOrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpicyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupOrderItem_GroupOrder_GroupOrderId",
                        column: x => x.GroupOrderId,
                        principalTable: "GroupOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupOrderItem_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_BranchId",
                table: "Menus",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_MenuItemId",
                table: "Components",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_MenuId",
                table: "Categories",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupOrder_CreatorId",
                table: "GroupOrder",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupOrder_TableId",
                table: "GroupOrder",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupOrderItem_GroupOrderId",
                table: "GroupOrderItem",
                column: "GroupOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupOrderItem_MenuItemId",
                table: "GroupOrderItem",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Menus_MenuId",
                table: "Categories",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_MenuItems_MenuItemId",
                table: "Components",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Branches_BranchId",
                table: "Menus",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Menus_MenuId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Components_MenuItems_MenuItemId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Branches_BranchId",
                table: "Menus");

            migrationBuilder.DropTable(
                name: "GroupOrderItem");

            migrationBuilder.DropTable(
                name: "GroupOrder");

            migrationBuilder.DropIndex(
                name: "IX_Menus_BranchId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Components_MenuItemId",
                table: "Components");

            migrationBuilder.DropIndex(
                name: "IX_Categories_MenuId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "GoogleMapsLocation",
                table: "Branches");
        }
    }
}
