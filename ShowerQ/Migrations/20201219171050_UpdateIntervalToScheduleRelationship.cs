using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class UpdateIntervalToScheduleRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intervals_Schedules_ScheduleId",
                table: "Intervals");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "Intervals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Intervals_Schedules_ScheduleId",
                table: "Intervals",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intervals_Schedules_ScheduleId",
                table: "Intervals");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "Intervals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Intervals_Schedules_ScheduleId",
                table: "Intervals",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
