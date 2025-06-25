using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class AddMezonUserIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MezonUserId",
                table: "AbpUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MezonUserId",
                table: "AbpUsers");
        }
    }
}
