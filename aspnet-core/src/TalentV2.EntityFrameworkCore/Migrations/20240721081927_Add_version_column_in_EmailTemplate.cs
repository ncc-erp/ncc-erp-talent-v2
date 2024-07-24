using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class Add_version_column_in_EmailTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "EmailTemplates",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "EmailTemplates");
        }
    }
}
