using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoShare.Server.Migrations
{
    public partial class V120 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Pictures",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Pictures");
        }
    }
}
