using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyApi.Migrations
{
    /// <inheritdoc />
    public partial class alterproducttb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTaxable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTaxable",
                table: "Products");
        }
    }
}
