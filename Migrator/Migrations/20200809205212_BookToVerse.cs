using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Migrator.Migrations
{
    public partial class BookToVerse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookId",
                table: "Verse",
                nullable: false
              );

            migrationBuilder.AlterColumn<byte[]>(
                name: "Testament",
                table: "Book",
                type: "string",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Verse_BookId",
                table: "Verse",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Verse_Book_BookId",
                table: "Verse",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verse_Book_BookId",
                table: "Verse");

            migrationBuilder.DropIndex(
                name: "IX_Verse_BookId",
                table: "Verse");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Verse");

            migrationBuilder.AlterColumn<int>(
                name: "Testament",
                table: "Book",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "string");
        }
    }
}
