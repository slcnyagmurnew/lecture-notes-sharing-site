using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Moon_.Migrations
{
    public partial class initalcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    DocumentId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    FileType = table.Column<string>(maxLength: 100, nullable: true),
                    DataFiles = table.Column<byte[]>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    ownerId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    CourseCode = table.Column<string>(nullable: true),
                    Lecturer = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.DocumentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
