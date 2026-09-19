using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tashrif.Context.Migrations
{
    /// <inheritdoc />
    public partial class FixChangedByCascadeBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_status_history_users_changed_by",
                table: "application_status_history");

            migrationBuilder.AddForeignKey(
                name: "FK_application_status_history_users_changed_by",
                table: "application_status_history",
                column: "changed_by",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_status_history_users_changed_by",
                table: "application_status_history");

            migrationBuilder.AddForeignKey(
                name: "FK_application_status_history_users_changed_by",
                table: "application_status_history",
                column: "changed_by",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
