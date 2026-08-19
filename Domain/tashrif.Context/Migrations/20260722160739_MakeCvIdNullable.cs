using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tashrif.Context.Migrations
{
    /// <inheritdoc />
    public partial class MakeCvIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applications_cvs_cv_id",
                table: "applications");

            migrationBuilder.AlterColumn<long>(
                name: "cv_id",
                table: "applications",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_applications_cvs_cv_id",
                table: "applications",
                column: "cv_id",
                principalTable: "cvs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applications_cvs_cv_id",
                table: "applications");

            migrationBuilder.AlterColumn<long>(
                name: "cv_id",
                table: "applications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_applications_cvs_cv_id",
                table: "applications",
                column: "cv_id",
                principalTable: "cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
