using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class AddAdministratorsToDormitory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "AspNetUsers",
                newName: "DormitoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DormitoryId1",
                table: "AspNetUsers",
                column: "DormitoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryId",
                table: "AspNetUsers",
                column: "DormitoryId",
                principalTable: "Dormitories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryId1",
                table: "AspNetUsers",
                column: "DormitoryId1",
                principalTable: "Dormitories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DormitoryId1",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DormitoryId1",
                table: "AspNetUsers",
                newName: "Gender");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryId",
                table: "AspNetUsers",
                column: "DormitoryId",
                principalTable: "Dormitories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
