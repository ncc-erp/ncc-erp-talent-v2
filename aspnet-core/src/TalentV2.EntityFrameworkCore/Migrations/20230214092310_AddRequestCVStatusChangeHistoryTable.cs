using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class AddRequestCVStatusChangeHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestCVStatusChangeHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    FromStatus = table.Column<int>(type: "integer", nullable: true),
                    TimeAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ToStatus = table.Column<int>(type: "integer", nullable: false),
                    RequestCVId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RequestCVStatusChangeHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestCVStatusChangeHistories_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestCVStatusChangeHistories_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestCVStatusChangeHistories_RequestCVs_RequestCVId",
                        column: x => x.RequestCVId,
                        principalTable: "RequestCVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestCVStatusChangeHistories_CreatorUserId",
                table: "RequestCVStatusChangeHistories",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCVStatusChangeHistories_LastModifierUserId",
                table: "RequestCVStatusChangeHistories",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCVStatusChangeHistories_RequestCVId",
                table: "RequestCVStatusChangeHistories",
                column: "RequestCVId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestCVStatusChangeHistories");
        }
    }
}
