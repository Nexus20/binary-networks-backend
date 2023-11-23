using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryNetworks.Infrastructure.Migrations
{
    public partial class MovedBackNeededColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BinaryNetworks");
        }
    }
}
