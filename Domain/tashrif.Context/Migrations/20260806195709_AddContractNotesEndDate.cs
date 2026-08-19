using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tashrif.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddContractNotesEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "contracts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "contracts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "contracts");
        }
    }
}
