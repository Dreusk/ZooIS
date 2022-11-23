using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animal_Species_SpeciesGuid",
                table: "Animal");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnimal_Animal_ChildrenGuid",
                table: "AnimalAnimal");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnimal_Animal_ParentsGuid",
                table: "AnimalAnimal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Animal",
                table: "Animal");

            migrationBuilder.RenameTable(
                name: "Animal",
                newName: "Animals");

            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "Animals",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Animal_SpeciesGuid",
                table: "Animals",
                newName: "IX_Animals_SpeciesGuid");

            migrationBuilder.AlterColumn<string>(
                name: "VernacularName",
                table: "Species",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Logs",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PageTitle",
                table: "Logs",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SpeciesGuid",
                table: "Animals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animals",
                table: "Animals",
                column: "Guid");

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Alerts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTag",
                columns: table => new
                {
                    Word = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTag", x => x.Word);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Supply = table.Column<long>(type: "bigint", nullable: true),
                    Demand = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.Guid);
                });

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

            migrationBuilder.CreateTable(
                name: "Characterization",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characterization", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Characterization_Animals_AnimalGuid",
                        column: x => x.AnimalGuid,
                        principalTable: "Animals",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characterization_Employees_AuthorGuid",
                        column: x => x.AuthorGuid,
                        principalTable: "Employees",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Name",
                columns: table => new
                {
                    EmployeeGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "AnimalFood",
                columns: table => new
                {
                    EatersGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FavouriteFoodGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalFood", x => new { x.EatersGuid, x.FavouriteFoodGuid });
                    table.ForeignKey(
                        name: "FK_AnimalFood_Animals_EatersGuid",
                        column: x => x.EatersGuid,
                        principalTable: "Animals",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalFood_Food_FavouriteFoodGuid",
                        column: x => x.FavouriteFoodGuid,
                        principalTable: "Food",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTagCharacterization",
                columns: table => new
                {
                    CharacterizationsGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsWord = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTagCharacterization", x => new { x.CharacterizationsGuid, x.TagsWord });
                    table.ForeignKey(
                        name: "FK_CharacterTagCharacterization_Characterization_CharacterizationsGuid",
                        column: x => x.CharacterizationsGuid,
                        principalTable: "Characterization",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterTagCharacterization_CharacterTag_TagsWord",
                        column: x => x.TagsWord,
                        principalTable: "CharacterTag",
                        principalColumn: "Word",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalFood_FavouriteFoodGuid",
                table: "AnimalFood",
                column: "FavouriteFoodGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Characterization_AnimalGuid",
                table: "Characterization",
                column: "AnimalGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Characterization_AuthorGuid",
                table: "Characterization",
                column: "AuthorGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTagCharacterization_TagsWord",
                table: "CharacterTagCharacterization",
                column: "TagsWord");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnimal_Animals_ChildrenGuid",
                table: "AnimalAnimal",
                column: "ChildrenGuid",
                principalTable: "Animals",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnimal_Animals_ParentsGuid",
                table: "AnimalAnimal",
                column: "ParentsGuid",
                principalTable: "Animals",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Species_SpeciesGuid",
                table: "Animals",
                column: "SpeciesGuid",
                principalTable: "Species",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnimal_Animals_ChildrenGuid",
                table: "AnimalAnimal");

            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnimal_Animals_ParentsGuid",
                table: "AnimalAnimal");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Species_SpeciesGuid",
                table: "Animals");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AnimalFood");

            migrationBuilder.DropTable(
                name: "CharacterTagCharacterization");

            migrationBuilder.DropTable(
                name: "Name");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "Characterization");

            migrationBuilder.DropTable(
                name: "CharacterTag");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Animals",
                table: "Animals");

            migrationBuilder.RenameTable(
                name: "Animals",
                newName: "Animal");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Animal",
                newName: "Nickname");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_SpeciesGuid",
                table: "Animal",
                newName: "IX_Animal_SpeciesGuid");

            migrationBuilder.AlterColumn<string>(
                name: "VernacularName",
                table: "Species",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "PageTitle",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<Guid>(
                name: "SpeciesGuid",
                table: "Animal",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animal",
                table: "Animal",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Animal_Species_SpeciesGuid",
                table: "Animal",
                column: "SpeciesGuid",
                principalTable: "Species",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnimal_Animal_ChildrenGuid",
                table: "AnimalAnimal",
                column: "ChildrenGuid",
                principalTable: "Animal",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnimal_Animal_ParentsGuid",
                table: "AnimalAnimal",
                column: "ParentsGuid",
                principalTable: "Animal",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
