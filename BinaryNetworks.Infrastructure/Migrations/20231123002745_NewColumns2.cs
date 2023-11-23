using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryNetworks.Infrastructure.Migrations
{
    public partial class NewColumns2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviewImageFileId",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviewImageFileId",
                table: "BinaryNetworks");
        }
    }
}
