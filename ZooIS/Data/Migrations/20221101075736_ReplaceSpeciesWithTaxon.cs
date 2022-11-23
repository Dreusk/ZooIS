using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class ReplaceSpeciesWithTaxon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Animals");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentGuid",
                table: "Species",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_ParentGuid",
                table: "Species",
                column: "ParentGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Species_Species_ParentGuid",
                table: "Species",
                column: "ParentGuid",
                principalTable: "Species",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_Species_ParentGuid",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_ParentGuid",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "ParentGuid",
                table: "Species");

            migrationBuilder.AddColumn<long>(
                name: "Age",
                table: "Animals",
                type: "bigint",
                nullable: true);
        }
    }
}
