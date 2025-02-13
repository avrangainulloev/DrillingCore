using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProjectEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasCamp",
                table: "Projects",
                newName: "HasCampOrHotel");

            migrationBuilder.RenameColumn(
                name: "Customer",
                table: "Projects",
                newName: "Client");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasCampOrHotel",
                table: "Projects",
                newName: "HasCamp");

            migrationBuilder.RenameColumn(
                name: "Client",
                table: "Projects",
                newName: "Customer");
        }
    }
}
