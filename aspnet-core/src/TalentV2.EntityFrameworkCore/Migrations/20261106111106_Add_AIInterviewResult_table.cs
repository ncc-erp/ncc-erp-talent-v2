using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class Add_AIInterviewResult_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIInterviewResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    SessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CandidateEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CandidateMezonUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CandidateName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CVId = table.Column<long>(type: "bigint", nullable: true),
                    RequestCVId = table.Column<long>(type: "bigint", nullable: true),
                    CoachName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ScenarioId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ScenarioTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InterviewedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DurationSeconds = table.Column<double>(type: "double precision", nullable: true),
                    OverallScore = table.Column<double>(type: "double precision", nullable: true),
                    CefrLevel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PronunciationScore = table.Column<double>(type: "double precision", nullable: true),
                    FluencyScore = table.Column<double>(type: "double precision", nullable: true),
                    VocabularyScore = table.Column<double>(type: "double precision", nullable: true),
                    GrammarScore = table.Column<double>(type: "double precision", nullable: true),
                    DomainScore = table.Column<double>(type: "double precision", nullable: true),
                    IeltsBand = table.Column<double>(type: "double precision", nullable: true),
                    ToeflScore = table.Column<double>(type: "double precision", nullable: true),
                    Report = table.Column<string>(type: "jsonb", nullable: true),
                    ProcessStatus = table.Column<byte>(type: "smallint", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_AIInterviewResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIInterviewResults_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIInterviewResults_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIInterviewResults_CVs_CVId",
                        column: x => x.CVId,
                        principalTable: "CVs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIInterviewResults_RequestCVs_RequestCVId",
                        column: x => x.RequestCVId,
                        principalTable: "RequestCVs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIInterviewResults_CandidateEmail",
                table: "AIInterviewResults",
                column: "CandidateEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AIInterviewResults_CreatorUserId",
                table: "AIInterviewResults",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AIInterviewResults_CVId",
                table: "AIInterviewResults",
                column: "CVId");

            migrationBuilder.CreateIndex(
                name: "IX_AIInterviewResults_LastModifierUserId",
                table: "AIInterviewResults",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AIInterviewResults_RequestCVId",
                table: "AIInterviewResults",
                column: "RequestCVId");

            migrationBuilder.CreateIndex(
                name: "IX_AIInterviewResults_SessionId",
                table: "AIInterviewResults",
                column: "SessionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIInterviewResults");
        }
    }
}
