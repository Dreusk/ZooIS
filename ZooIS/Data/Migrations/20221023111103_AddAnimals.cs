using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddAnimals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VernacularName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Animal",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Age = table.Column<long>(type: "bigint", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpeciesGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animal", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Animal_Species_SpeciesGuid",
                        column: x => x.SpeciesGuid,
                        principalTable: "Species",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalAnimal",
                columns: table => new
                {
                    ChildrenGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentsGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalAnimal", x => new { x.ChildrenGuid, x.ParentsGuid });
                    table.ForeignKey(
                        name: "FK_AnimalAnimal_Animal_ChildrenGuid",
                        column: x => x.ChildrenGuid,
                        principalTable: "Animal",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalAnimal_Animal_ParentsGuid",
                        column: x => x.ParentsGuid,
                        principalTable: "Animal",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animal_SpeciesGuid",
                table: "Animal",
                column: "SpeciesGuid");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalAnimal_ParentsGuid",
                table: "AnimalAnimal",
                column: "ParentsGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalAnimal");

            migrationBuilder.DropTable(
                name: "Animal");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
