using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class CandidateLanguages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CandidateLanguage",
                table: "CVs",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CandidateLanguages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Alias = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ColorCode = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateLanguages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateLanguages");

            migrationBuilder.DropColumn(
                name: "CandidateLanguage",
                table: "CVs");
        }
    }
}
