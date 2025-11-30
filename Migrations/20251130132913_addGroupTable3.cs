using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyApi.Migrations
{
    /// <inheritdoc />
    public partial class addGroupTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryDocuments_Group_GroupId",
                table: "TemporaryDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "TemporaryDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "TemporaryDocuments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryDocuments_Group_GroupId",
                table: "TemporaryDocuments",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");
        }
    }
}
