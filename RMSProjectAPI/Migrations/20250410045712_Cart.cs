using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMSProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class Cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MenuItemName",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MenuItemSizeId",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MenuItemSizeId",
                table: "CartItems",
                column: "MenuItemSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_MenuItemSizes_MenuItemSizeId",
                table: "CartItems",
                column: "MenuItemSizeId",
                principalTable: "MenuItemSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_MenuItemSizes_MenuItemSizeId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_MenuItemSizeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "MenuItemName",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "MenuItemSizeId",
                table: "CartItems");
        }
    }
}
