using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class AddGeneologyReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create procedure GetGeneology @animal uniqueidentifier\r\nAS\r\nwith cte as (\r\nselect * \r\nfrom AnimalAnimal as T\r\nwhere T.ChildrenGuid = @animal OR T.ParentsGuid = @animal\r\nunion all\r\nselect T.* \r\nfrom AnimalAnimal as T\r\ninner join cte\r\n on (t.ChildrenGuid = cte.ParentsGuid OR t.ParentsGuid = cte.ChildrenGuid)\r\n and (t.ChildrenGuid != cte.ChildrenGuid and t.ParentsGuid = cte.ParentsGuid))\r\nSELECT *\r\nFROM cte;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop procedure GetGeneology");
        }
    }
}
