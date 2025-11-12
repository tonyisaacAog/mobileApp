using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyApi.Migrations
{
    /// <inheritdoc />
    public partial class hema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemporaryDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TotalDiscount = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    ExtraDiscount = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TotalVAT = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerGovernate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerStreet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerBuilding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    IsCoupon = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporaryDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemporaryDocuments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TemporaryDocuments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TemporaryDocuments_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TemporaryDocuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TemporaryDocumentLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    NetTotal = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    VAT = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporaryDocumentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemporaryDocumentLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemporaryDocumentLines_TemporaryDocuments_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "TemporaryDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocumentLines_ProductId",
                table: "TemporaryDocumentLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocumentLines_ReceiptId",
                table: "TemporaryDocumentLines",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocuments_BranchId",
                table: "TemporaryDocuments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocuments_CompanyId",
                table: "TemporaryDocuments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocuments_DeviceId",
                table: "TemporaryDocuments",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocuments_UserId",
                table: "TemporaryDocuments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporaryDocumentLines");

            migrationBuilder.DropTable(
                name: "TemporaryDocuments");
        }
    }
}
