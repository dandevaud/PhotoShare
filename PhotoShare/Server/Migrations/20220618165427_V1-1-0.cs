using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoShare.Server.Migrations
{
    public partial class V110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupKeys",
                table: "GroupKeys");

            migrationBuilder.DropColumn(
                name: "KeyType",
                table: "GroupKeys");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "GroupKeys",
                newName: "AdminKey");

            migrationBuilder.AddColumn<byte[]>(
                name: "IV",
                table: "Pictures",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "fileName",
                table: "Pictures",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "EncryptionKey",
                table: "GroupKeys",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupKeys",
                table: "GroupKeys",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupKeys",
                table: "GroupKeys");

            migrationBuilder.DropColumn(
                name: "IV",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "fileName",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "EncryptionKey",
                table: "GroupKeys");

            migrationBuilder.RenameColumn(
                name: "AdminKey",
                table: "GroupKeys",
                newName: "Key");

            migrationBuilder.AddColumn<int>(
                name: "KeyType",
                table: "GroupKeys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupKeys",
                table: "GroupKeys",
                columns: new[] { "GroupId", "KeyType" });
        }
    }
}
