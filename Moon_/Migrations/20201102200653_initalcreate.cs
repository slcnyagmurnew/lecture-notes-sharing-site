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
                    Category = table.Column<int>(nullable: false),
                    CourseCode = table.Column<string>(nullable: true),
                    Lecturer = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.DocumentId);
                });
                
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    name = table.Column<string>(nullable: true),
                    surname = table.Column<string>(nullable: true),
                    id = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: true),
                    department = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
