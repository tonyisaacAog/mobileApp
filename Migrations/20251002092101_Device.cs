using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyApi.Migrations
{
    /// <inheritdoc />
    public partial class Device : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DeviceId",
                table: "Documents",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Devices_DeviceId",
                table: "Documents",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Devices_DeviceId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DeviceId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Documents");
        }
    }
}
