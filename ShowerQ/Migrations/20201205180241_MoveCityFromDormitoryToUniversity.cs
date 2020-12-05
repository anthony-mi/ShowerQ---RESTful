using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class MoveCityFromDormitoryToUniversity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dormitories_Cities_CityId",
                table: "Dormitories");

            migrationBuilder.DropIndex(
                name: "IX_Dormitories_CityId",
                table: "Dormitories");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Dormitories");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Universities",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_CityId",
                table: "Universities",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Universities_Cities_CityId",
                table: "Universities",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Universities_Cities_CityId",
                table: "Universities");

            migrationBuilder.DropIndex(
                name: "IX_Universities_CityId",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Universities");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Dormitories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dormitories_CityId",
                table: "Dormitories",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dormitories_Cities_CityId",
                table: "Dormitories",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
