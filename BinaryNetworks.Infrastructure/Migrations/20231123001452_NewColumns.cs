using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryNetworks.Infrastructure.Migrations
{
    public partial class NewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NetworkFileId",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetworkFileId",
                table: "BinaryNetworks");
        }
    }
}
