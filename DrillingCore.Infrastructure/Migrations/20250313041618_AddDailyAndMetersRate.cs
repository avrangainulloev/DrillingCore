using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyAndMetersRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DailyRate",
                table: "Participants",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MeterRate",
                table: "Participants",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyRate",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "MeterRate",
                table: "Participants");
        }
    }
}
