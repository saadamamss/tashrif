using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tashrif.Context.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndividualProfileCvFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cv_file",
                table: "individual_profiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cv_file",
                table: "individual_profiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
