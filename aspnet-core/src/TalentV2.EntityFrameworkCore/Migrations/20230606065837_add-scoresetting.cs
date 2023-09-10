using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class addscoresetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoreSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    SubPositionId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_ScoreSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreSettings_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoreSettings_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoreSettings_SubPositions_SubPositionId",
                        column: x => x.SubPositionId,
                        principalTable: "SubPositions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScoreRanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    ScoreFrom = table.Column<int>(type: "integer", nullable: false),
                    ScoreTo = table.Column<int>(type: "integer", nullable: false),
                    ScoreSettingID = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_ScoreRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreRanges_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoreRanges_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoreRanges_ScoreSettings_ScoreSettingID",
                        column: x => x.ScoreSettingID,
                        principalTable: "ScoreSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreRanges_CreatorUserId",
                table: "ScoreRanges",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreRanges_LastModifierUserId",
                table: "ScoreRanges",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreRanges_ScoreSettingID",
                table: "ScoreRanges",
                column: "ScoreSettingID");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreSettings_CreatorUserId",
                table: "ScoreSettings",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreSettings_LastModifierUserId",
                table: "ScoreSettings",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreSettings_SubPositionId",
                table: "ScoreSettings",
                column: "SubPositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreRanges");

            migrationBuilder.DropTable(
                name: "ScoreSettings");
        }
    }
}
