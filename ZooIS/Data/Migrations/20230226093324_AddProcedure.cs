using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
            
            migrationBuilder.Sql("create proc taxon_GetHierarchy\r\n\t@rootGuid uniqueidentifier\r\nas\r\n\twith rec_taxons as (\r\n\tselect root.*\r\n\tfrom dbo.Taxons root\r\n\twhere root.Guid = @rootGuid\r\n\tunion all\r\n\tselect taxons.*\r\n\tfrom rec_taxons\r\n\tjoin dbo.Taxons\r\n\t  on taxons.Guid = rec_taxons.ParentGuid\r\n\t)\r\n\tselect *\r\n\tfrom rec_taxons\r\n\torder by Rank desc;\r\ngo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.Sql("drop procedure taxon_GetHierarchy;");
        }
    }
}
