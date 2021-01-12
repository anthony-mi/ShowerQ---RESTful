using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class AddPropertyToDormitory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DaysBeforePrioritiesNormalization",
                table: "Dormitories",
                type: "int",
                nullable: false,
                defaultValue: 14);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysBeforePrioritiesNormalization",
                table: "Dormitories");
        }
    }
}
