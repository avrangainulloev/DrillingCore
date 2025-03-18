using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStartDateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateAdded",
                table: "Participants",
                newName: "DateAdded1");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Participants",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Participants");

            migrationBuilder.RenameColumn(
                name: "DateAdded1",
                table: "Participants",
                newName: "DateAdded");
        }
    }
}
