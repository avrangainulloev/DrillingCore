using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FLHAGetForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FLHAForms_CreatorId",
                table: "FLHAForms",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_FLHAForms_Users_CreatorId",
                table: "FLHAForms",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FLHAForms_Users_CreatorId",
                table: "FLHAForms");

            migrationBuilder.DropIndex(
                name: "IX_FLHAForms_CreatorId",
                table: "FLHAForms");
        }
    }
}
