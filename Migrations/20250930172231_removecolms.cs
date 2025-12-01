using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyApi.Migrations
{
    /// <inheritdoc />
    public partial class removecolms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ReceiptNumber",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "InitialCreditTaxSerial",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "InitialDebitTaxSerial",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "InitialInvoiceTaxSerial",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "TaxCode",
                table: "Companies",
                newName: "TaxNumber");

            migrationBuilder.RenameColumn(
                name: "ERPClientSecret",
                table: "Companies",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "ERPClientID",
                table: "Companies",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ActivityCode",
                table: "Companies",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Serial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_BranchId",
                table: "Devices",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Code",
                table: "Devices",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Name",
                table: "Devices",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "TaxNumber",
                table: "Companies",
                newName: "TaxCode");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Companies",
                newName: "ERPClientSecret");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Companies",
                newName: "ERPClientID");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Companies",
                newName: "ActivityCode");

            migrationBuilder.AddColumn<int>(
                name: "InitialCreditTaxSerial",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InitialDebitTaxSerial",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InitialInvoiceTaxSerial",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReceiptNumber",
                table: "Documents",
                column: "ReceiptNumber",
                unique: true);
        }
    }
}
