using Microsoft.EntityFrameworkCore.Migrations;

namespace VigilantKJV.Migrations
{
    public partial class AddEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_Book_BookId",
                table: "Chapter");

            migrationBuilder.DropForeignKey(
                name: "FK_Verse_Book_BookId",
                table: "Verse");

            migrationBuilder.DropForeignKey(
                name: "FK_Verse_Chapter_ChapterId",
                table: "Verse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Verse",
                table: "Verse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapter",
                table: "Chapter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Book");

            migrationBuilder.RenameTable(
                name: "Verse",
                newName: "Verses");

            migrationBuilder.RenameTable(
                name: "Chapter",
                newName: "Chapters");

            migrationBuilder.RenameTable(
                name: "Book",
                newName: "Books");

            migrationBuilder.RenameIndex(
                name: "IX_Verse_ChapterId",
                table: "Verses",
                newName: "IX_Verses_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_Verse_BookId",
                table: "Verses",
                newName: "IX_Verses_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapter_BookId",
                table: "Chapters",
                newName: "IX_Chapters_BookId");

            migrationBuilder.AddColumn<string>(
                name: "BookName",
                table: "Books",
                type: "nvarchar",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verses",
                table: "Verses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapters",
                table: "Chapters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verses_Books_BookId",
                table: "Verses",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verses_Chapters_ChapterId",
                table: "Verses",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_Verses_Books_BookId",
                table: "Verses");

            migrationBuilder.DropForeignKey(
                name: "FK_Verses_Chapters_ChapterId",
                table: "Verses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Verses",
                table: "Verses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapters",
                table: "Chapters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookName",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "Verses",
                newName: "Verse");

            migrationBuilder.RenameTable(
                name: "Chapters",
                newName: "Chapter");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "Book");

            migrationBuilder.RenameIndex(
                name: "IX_Verses_ChapterId",
                table: "Verse",
                newName: "IX_Verse_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_Verses_BookId",
                table: "Verse",
                newName: "IX_Verse_BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_BookId",
                table: "Chapter",
                newName: "IX_Chapter_BookId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Book",
                type: "nvarchar",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verse",
                table: "Verse",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapter",
                table: "Chapter",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book",
                table: "Book",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_Book_BookId",
                table: "Chapter",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verse_Book_BookId",
                table: "Verse",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verse_Chapter_ChapterId",
                table: "Verse",
                column: "ChapterId",
                principalTable: "Chapter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
