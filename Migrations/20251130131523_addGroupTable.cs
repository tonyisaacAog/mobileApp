using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyApi.Migrations
{
    /// <inheritdoc />
    public partial class addGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "TemporaryDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemporaryDocuments_GroupId",
                table: "TemporaryDocuments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryDocuments_Group_GroupId",
                table: "TemporaryDocuments",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryDocuments_Group_GroupId",
                table: "TemporaryDocuments");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropIndex(
                name: "IX_TemporaryDocuments_GroupId",
                table: "TemporaryDocuments");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "TemporaryDocuments");
        }
    }
}
