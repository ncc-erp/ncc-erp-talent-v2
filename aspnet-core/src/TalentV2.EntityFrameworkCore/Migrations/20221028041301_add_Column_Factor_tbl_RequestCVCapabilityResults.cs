using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class add_Column_Factor_tbl_RequestCVCapabilityResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Factor",
                table: "RequestCVCapabilityResults",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Factor",
                table: "RequestCVCapabilityResults");
        }
    }
}
