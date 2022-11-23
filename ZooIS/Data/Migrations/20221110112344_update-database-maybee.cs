using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class updatedatabasemaybee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Species_SpeciesGuid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Species_Species_ParentGuid",
                table: "Species");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Species",
                table: "Species");

            migrationBuilder.RenameTable(
                name: "Species",
                newName: "Taxons");

            migrationBuilder.RenameIndex(
                name: "IX_Species_ParentGuid",
                table: "Taxons",
                newName: "IX_Taxons_ParentGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Taxons",
                table: "Taxons",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Taxons_SpeciesGuid",
                table: "Animals",
                column: "SpeciesGuid",
                principalTable: "Taxons",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Taxons_Taxons_ParentGuid",
                table: "Taxons",
                column: "ParentGuid",
                principalTable: "Taxons",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Taxons_SpeciesGuid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Taxons_Taxons_ParentGuid",
                table: "Taxons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Taxons",
                table: "Taxons");

            migrationBuilder.RenameTable(
                name: "Taxons",
                newName: "Species");

            migrationBuilder.RenameIndex(
                name: "IX_Taxons_ParentGuid",
                table: "Species",
                newName: "IX_Species_ParentGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Species",
                table: "Species",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Species_SpeciesGuid",
                table: "Animals",
                column: "SpeciesGuid",
                principalTable: "Species",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Species_ParentGuid",
                table: "Species",
                column: "ParentGuid",
                principalTable: "Species",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
