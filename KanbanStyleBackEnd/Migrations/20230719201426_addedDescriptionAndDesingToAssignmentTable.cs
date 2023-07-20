using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanStyleBackEnd.Migrations
{
    public partial class addedDescriptionAndDesingToAssignmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionURL",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DesignURL",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionURL",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "DesignURL",
                table: "Assignments");
        }
    }
}
