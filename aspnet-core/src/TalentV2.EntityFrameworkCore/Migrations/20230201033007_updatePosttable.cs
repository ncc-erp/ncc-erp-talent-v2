using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class updatePosttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterEmailAddress",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PosterName",
                table: "Posts");

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUser",
                table: "Posts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Posts",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Posts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUser",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "PosterEmailAddress",
                table: "Posts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PosterName",
                table: "Posts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
