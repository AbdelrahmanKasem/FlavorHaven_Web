using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMSProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Branches_BranchId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_BranchId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Tables");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Tables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tables_BranchId",
                table: "Tables",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Branches_BranchId",
                table: "Tables",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
