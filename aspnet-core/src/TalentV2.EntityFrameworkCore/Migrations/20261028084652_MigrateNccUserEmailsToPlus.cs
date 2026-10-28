using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentV2.Migrations
{
    public partial class MigrateNccUserEmailsToPlus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EF runs these operations in one transaction. Block writes during validation/update.
            migrationBuilder.Sql(@"LOCK TABLE ""AbpUsers"" IN SHARE ROW EXCLUSIVE MODE;");

            // This migration-owned table intentionally has no domain entity or foreign key:
            // rollback must retain its evidence even if a user is subsequently hard-deleted.
            migrationBuilder.CreateTable(
                name: "UserEmailDomainMigrationBackup",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    OldEmailAddress = table.Column<string>(
                        type: "character varying(256)", maxLength: 256, nullable: false),
                    OldNormalizedEmailAddress = table.Column<string>(
                        type: "character varying(256)", maxLength: 256, nullable: false),
                    NewEmailAddress = table.Column<string>(
                        type: "character varying(256)", maxLength: 256, nullable: false),
                    NewNormalizedEmailAddress = table.Column<string>(
                        type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_UserEmailDomainMigrationBackup", x => x.UserId));

            migrationBuilder.Sql(@"
                INSERT INTO ""UserEmailDomainMigrationBackup""
                (
                    ""UserId"", ""TenantId"", ""OldEmailAddress"", ""OldNormalizedEmailAddress"",
                    ""NewEmailAddress"", ""NewNormalizedEmailAddress""
                )
                SELECT
                    ""Id"", ""TenantId"", ""EmailAddress"", ""NormalizedEmailAddress"",
                    left(""EmailAddress"", length(""EmailAddress"") - length('@ncc.asia')) || '@ncc.plus',
                    upper(left(""EmailAddress"", length(""EmailAddress"") - length('@ncc.asia')) || '@ncc.plus')
                FROM ""AbpUsers""
                WHERE NOT ""IsDeleted""
                  AND right(lower(""EmailAddress""), length('@ncc.asia')) = '@ncc.asia';

                DO $migration$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM ""UserEmailDomainMigrationBackup"" a
                        JOIN ""UserEmailDomainMigrationBackup"" b
                          ON a.""UserId"" < b.""UserId""
                         AND a.""TenantId"" IS NOT DISTINCT FROM b.""TenantId""
                         AND a.""NewNormalizedEmailAddress"" = b.""NewNormalizedEmailAddress""
                    ) OR EXISTS (
                        SELECT 1
                        FROM ""UserEmailDomainMigrationBackup"" b
                        JOIN ""AbpUsers"" u
                          ON u.""TenantId"" IS NOT DISTINCT FROM b.""TenantId""
                         AND u.""Id"" <> b.""UserId""
                         AND NOT u.""IsDeleted""
                         AND (
                             upper(u.""EmailAddress"") = b.""NewNormalizedEmailAddress""
                             OR upper(u.""NormalizedEmailAddress"") = b.""NewNormalizedEmailAddress""
                         )
                    ) THEN
                        RAISE EXCEPTION 'NCC email migration aborted: target email conflict within a tenant. No users were migrated.';
                    END IF;
                END $migration$;

                UPDATE ""AbpUsers"" u
                SET ""EmailAddress"" = b.""NewEmailAddress"",
                    ""NormalizedEmailAddress"" = b.""NewNormalizedEmailAddress""
                FROM ""UserEmailDomainMigrationBackup"" b
                WHERE u.""Id"" = b.""UserId"";

                COMMENT ON TABLE ""UserEmailDomainMigrationBackup"" IS
                    'Rollback data for 20261028084652_MigrateNccUserEmailsToPlus. Keep until this migration is reverted.';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                LOCK TABLE ""AbpUsers"", ""UserEmailDomainMigrationBackup"" IN SHARE ROW EXCLUSIVE MODE;

                DO $migration$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM ""UserEmailDomainMigrationBackup"" b
                        JOIN ""AbpUsers"" u
                          ON u.""Id"" = b.""UserId""
                        WHERE u.""EmailAddress"" IS DISTINCT FROM b.""NewEmailAddress""
                           OR u.""NormalizedEmailAddress"" IS DISTINCT FROM b.""NewNormalizedEmailAddress""
                           OR u.""TenantId"" IS DISTINCT FROM b.""TenantId""
                    ) THEN
                        RAISE EXCEPTION 'NCC email rollback aborted: a migrated user email, normalization or tenant changed. No users were restored.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM ""UserEmailDomainMigrationBackup"" b
                        JOIN ""AbpUsers"" restoring
                          ON restoring.""Id"" = b.""UserId""
                        JOIN ""AbpUsers"" other
                          ON other.""TenantId"" IS NOT DISTINCT FROM b.""TenantId""
                         AND other.""Id"" <> b.""UserId""
                         AND NOT other.""IsDeleted""
                         AND (
                             upper(other.""EmailAddress"") IN (
                                 upper(b.""OldEmailAddress""), upper(b.""OldNormalizedEmailAddress"")
                             )
                             OR upper(other.""NormalizedEmailAddress"") IN (
                                 upper(b.""OldEmailAddress""), upper(b.""OldNormalizedEmailAddress"")
                             )
                         )
                    ) THEN
                        RAISE EXCEPTION 'NCC email rollback aborted: original email conflict within a tenant. No users were restored.';
                    END IF;
                END $migration$;

                UPDATE ""AbpUsers"" u
                SET ""EmailAddress"" = b.""OldEmailAddress"",
                    ""NormalizedEmailAddress"" = b.""OldNormalizedEmailAddress""
                FROM ""UserEmailDomainMigrationBackup"" b
                WHERE u.""Id"" = b.""UserId"";
            ");
            migrationBuilder.DropTable(name: "UserEmailDomainMigrationBackup");
        }
    }
}
