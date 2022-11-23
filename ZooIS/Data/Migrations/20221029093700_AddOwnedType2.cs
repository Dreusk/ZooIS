using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddOwnedType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Name_FamilyName",
                table: "Employees",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name_GivenName",
                table: "Employees",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name_ThirdName",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_FamilyName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Name_GivenName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Name_ThirdName",
                table: "Employees");

            migrationBuilder.CreateTable(
                name: "Name",
                columns: table => new
                {
                    EmployeeGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ThirdName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Name", x => x.EmployeeGuid);
                    table.ForeignKey(
                        name: "FK_Name_Employees_EmployeeGuid",
                        column: x => x.EmployeeGuid,
                        principalTable: "Employees",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
