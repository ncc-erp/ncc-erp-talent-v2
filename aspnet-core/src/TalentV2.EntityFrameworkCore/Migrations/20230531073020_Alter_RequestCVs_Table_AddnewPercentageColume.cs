using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class Alter_RequestCVs_Table_AddnewPercentageColume : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Percentage",
                table: "RequestCVs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "RequestCVs");
        }
    }
}
