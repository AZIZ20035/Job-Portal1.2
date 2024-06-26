﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JopPortal.Migrations
{
    public partial class job13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "user");
        }
    }
}
