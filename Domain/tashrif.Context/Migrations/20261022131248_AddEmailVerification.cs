using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace tashrif.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- D12 (step 1): normalize every stored email to lowercase + trimmed ----
            // Runs BEFORE the unique index so that "A@x.com" and "a@x.com" collapse
            // into one group instead of two. An index over duplicates fails to apply.
            migrationBuilder.Sql(@"
                UPDATE ""users""
                SET ""email"" = lower(btrim(""email""))
                WHERE ""email"" IS DISTINCT FROM lower(btrim(""email""));");

            // ---- D12 (step 2): resolve duplicate emails that already exist ----
            // The dev DB shipped with two duplicate groups. A unique index cannot be
            // created over them, and deleting users is not an option (their rows carry
            // profiles/applications). So: the LOWEST id in each group keeps the address,
            // and every later row gets a deterministic, collision-free suffix
            // ("<email>+dup<id>").
            //
            // The loop (rather than one bulk UPDATE) is deliberate: a suffix could in
            // principle collide with an address that already exists, and the loop keeps
            // re-checking until the table is genuinely duplicate-free. Each iteration
            // strictly lengthens one row's email, so it terminates.
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    dup_id bigint;
                BEGIN
                    LOOP
                        SELECT u.""Id""
                        INTO dup_id
                        FROM ""users"" u
                        WHERE EXISTS (
                            SELECT 1 FROM ""users"" o
                            WHERE o.""email"" = u.""email"" AND o.""Id"" <> u.""Id""
                        )
                        ORDER BY u.""Id"" DESC
                        LIMIT 1;

                        EXIT WHEN dup_id IS NULL;

                        UPDATE ""users""
                        SET ""email"" = ""email"" || '+dup' || dup_id::text
                        WHERE ""Id"" = dup_id;
                    END LOOP;
                END $$;");

            migrationBuilder.AddColumn<bool>(
                name: "is_email_verified",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // ---- D9: backfill existing users as VERIFIED ----
            // They registered before this feature existed; locking them out would be a
            // regression with no way for them to receive a token. The column DEFAULT
            // stays false so genuinely new registrations start unverified.
            migrationBuilder.Sql(@"
                UPDATE ""users"" SET ""is_email_verified"" = true;");

            migrationBuilder.CreateTable(
                name: "email_verification_tokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_error = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_verification_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_email_verification_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_verification_tokens_token_hash",
                table: "email_verification_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_verification_tokens_user_id",
                table: "email_verification_tokens",
                column: "user_id");

            // ---- D12 (step 3): normalize on write, so the unique index is case-insensitive ----
            // A plain UNIQUE index on a text column is CASE-SENSITIVE: without this trigger
            // "A@x.com" and "a@x.com" would both be insertable, and the "one account per
            // email" invariant would be trivially bypassed by changing case.
            //
            // A BEFORE trigger is used (rather than `citext` or a functional index) because
            // it needs no PostgreSQL extension — which matters, since the target database
            // may not have citext installed and CREATE EXTENSION needs elevated privileges.
            // The trigger guarantees the invariant for EVERY write path (registration,
            // admin profile update, raw SQL, future services) rather than relying on each
            // caller to remember to lowercase.
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION tashrif_normalize_user_email()
                RETURNS trigger AS $$
                BEGIN
                    NEW.""email"" := lower(btrim(NEW.""email""));
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                DROP TRIGGER IF EXISTS trg_users_normalize_email ON ""users"";

                CREATE TRIGGER trg_users_normalize_email
                BEFORE INSERT OR UPDATE OF ""email"" ON ""users""
                FOR EACH ROW EXECUTE FUNCTION tashrif_normalize_user_email();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // NOTE: the D12 data repair above (lowercasing + "+dup<id>" suffixes) is
            // deliberately NOT reverted — the original addresses are not recoverable.
            // Down() only removes the schema objects this migration added.

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_users_normalize_email ON ""users"";
                DROP FUNCTION IF EXISTS tashrif_normalize_user_email();");

            migrationBuilder.DropTable(
                name: "email_verification_tokens");

            migrationBuilder.DropIndex(
                name: "IX_users_email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_email_verified",
                table: "users");
        }
    }
}
