using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddPictureToAnimall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PicturePath",
                table: "Animals",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicturePath",
                table: "Animals");
        }
    }
}
