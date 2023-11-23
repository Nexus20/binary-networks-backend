using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryNetworks.Infrastructure.Migrations
{
    public partial class MoveAllToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BinaryNetworkJson",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "PreviewImage",
                table: "BinaryNetworks",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinaryNetworkJson",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "PreviewImage",
                table: "BinaryNetworks");
        }
    }
}
