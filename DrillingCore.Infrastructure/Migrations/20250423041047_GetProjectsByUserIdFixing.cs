using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetProjectsByUserIdFixing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Projects_ProjectId1",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_ProjectId1",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "Participants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId1",
                table: "Participants",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ProjectId1",
                table: "Participants",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Projects_ProjectId1",
                table: "Participants",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
