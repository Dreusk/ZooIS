using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddRankToTaxons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Taxons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Taxons");
        }
    }
}
