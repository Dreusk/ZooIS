using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddOwnedType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.AddColumn<int>(
                name: "Status_Health",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status_Health",
                table: "Animals");

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    AnimalGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.AnimalGuid);
                    table.ForeignKey(
                        name: "FK_Status_Animals_AnimalGuid",
                        column: x => x.AnimalGuid,
                        principalTable: "Animals",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
