using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BobaTeaFace.Data.Migrations
{
    public partial class course : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    ChildImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhoQ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhoA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatQ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhenQ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhenA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhyQ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhyA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhereQ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhereA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HowQ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HowA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
