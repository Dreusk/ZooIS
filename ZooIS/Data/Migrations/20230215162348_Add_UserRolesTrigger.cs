using Microsoft.EntityFrameworkCore.Migrations;

namespace ZooIS.Data.Migrations
{
    public partial class Add_UserRolesTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create trigger UserRoles_Insert on dbo.RoleUser\r\nafter insert as begin\r\n\tinsert into AspNetUserRoles(UserId, RoleId)\r\n\tselect UsersId, RolesId\r\n\tfrom inserted\r\nend;\r\ngo");
            migrationBuilder.Sql("create trigger UserRoles_Delete on dbo.RoleUser\r\nafter insert as begin\r\n\tdelete from AspNetUserRoles\r\n\tfrom AspNetUserRoles asp\r\n\tjoin deleted del\r\n  on asp.UserId = del.UsersId\r\n  and asp.RoleId = del.RolesId\r\nend;\r\ngo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop trigger UserRoles_Insert");
            migrationBuilder.Sql("drop trigger UserRoles_Delete");
        }
    }
}
