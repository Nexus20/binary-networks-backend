using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryNetworks.Infrastructure.Migrations
{
    public partial class CleanColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "NetworkBlobName",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "NetworkFileId",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "NetworkFileUrl",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "PreviewImageBlobName",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "PreviewImageFileId",
                table: "BinaryNetworks");

            migrationBuilder.DropColumn(
                name: "PreviewImageUrl",
                table: "BinaryNetworks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkBlobName",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkFileId",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NetworkFileUrl",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewImageBlobName",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewImageFileId",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewImageUrl",
                table: "BinaryNetworks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
