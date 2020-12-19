using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class ChangeIntervalTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Interval",
                table: "Interval");

            migrationBuilder.RenameTable(
                name: "Interval",
                newName: "Intervals");

            migrationBuilder.RenameIndex(
                name: "IX_Interval_ScheduleId",
                table: "Intervals",
                newName: "IX_Intervals_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Intervals",
                table: "Intervals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Intervals_Schedules_ScheduleId",
                table: "Intervals",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dormitories_Schedules_CurrentScheduleId",
                table: "Dormitories");

            migrationBuilder.DropForeignKey(
                name: "FK_Intervals_Schedules_ScheduleId",
                table: "Intervals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Intervals",
                table: "Intervals");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedule");

            migrationBuilder.RenameTable(
                name: "Intervals",
                newName: "Interval");

            migrationBuilder.RenameIndex(
                name: "IX_Intervals_ScheduleId",
                table: "Interval",
                newName: "IX_Interval_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interval",
                table: "Interval",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dormitories_Schedule_CurrentScheduleId",
                table: "Dormitories",
                column: "CurrentScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interval_Schedule_ScheduleId",
                table: "Interval",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
