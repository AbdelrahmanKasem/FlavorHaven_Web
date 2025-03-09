using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMSProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class GeneratingQRCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QrCode",
                table: "Tables",
                newName: "QrCodeUrl");

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "Tables",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "Tables");

            migrationBuilder.RenameColumn(
                name: "QrCodeUrl",
                table: "Tables",
                newName: "QrCode");
        }
    }
}
