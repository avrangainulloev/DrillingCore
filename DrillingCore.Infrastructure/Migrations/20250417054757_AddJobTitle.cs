using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormSignatures_ParticipantId",
                table: "FormSignatures",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSignatures_Participants_ParticipantId",
                table: "FormSignatures",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSignatures_Participants_ParticipantId",
                table: "FormSignatures");

            migrationBuilder.DropIndex(
                name: "IX_FormSignatures_ParticipantId",
                table: "FormSignatures");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Users");
        }
    }
}
