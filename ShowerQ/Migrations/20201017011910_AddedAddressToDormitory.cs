using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShowerQ.Migrations
{
    public partial class AddedAddressToDormitory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Address", table: "Dormitories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
