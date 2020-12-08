using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class UpdateUsersDbStorageType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_Tenant_DormitoryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Tenant_DormitoryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Tenant_DormitoryId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tenant_DormitoryId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Tenant_DormitoryId",
                table: "AspNetUsers",
                column: "Tenant_DormitoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_Tenant_DormitoryId",
                table: "AspNetUsers",
                column: "Tenant_DormitoryId",
                principalTable: "Dormitories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
