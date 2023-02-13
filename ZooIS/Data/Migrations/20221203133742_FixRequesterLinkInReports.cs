using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class FixRequesterLinkInReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequesterId",
                table: "Reports",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_RequesterId",
                table: "Reports",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_RequesterId",
                table: "Reports",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_RequesterId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_RequesterId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Reports");
        }
    }
}
