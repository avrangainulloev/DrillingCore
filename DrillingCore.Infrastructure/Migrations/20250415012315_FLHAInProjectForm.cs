using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FLHAInProjectForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FLHAFormHazards_FLHAForms_FLHAFormId",
                table: "FLHAFormHazards");

            migrationBuilder.DropForeignKey(
                name: "FK_FLHAForms_Users_CreatorId",
                table: "FLHAForms");

            migrationBuilder.DropForeignKey(
                name: "FK_FormParticipants_FLHAForms_FLHAFormId",
                table: "FormParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_FormPhotos_FLHAForms_FLHAFormId",
                table: "FormPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_FormSignatures_FLHAForms_FLHAFormId",
                table: "FormSignatures");

            migrationBuilder.DropIndex(
                name: "IX_FormSignatures_FLHAFormId",
                table: "FormSignatures");

            migrationBuilder.DropIndex(
                name: "IX_FormPhotos_FLHAFormId",
                table: "FormPhotos");

            migrationBuilder.DropIndex(
                name: "IX_FormParticipants_FLHAFormId",
                table: "FormParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FLHAForms",
                table: "FLHAForms");

            migrationBuilder.DropIndex(
                name: "IX_FLHAForms_CreatorId",
                table: "FLHAForms");

            migrationBuilder.DropColumn(
                name: "FLHAFormId",
                table: "FormSignatures");

            migrationBuilder.DropColumn(
                name: "FLHAFormId",
                table: "FormPhotos");

            migrationBuilder.DropColumn(
                name: "FLHAFormId",
                table: "FormParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FLHAForms");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "FLHAForms");

            migrationBuilder.DropColumn(
                name: "DateFilled",
                table: "FLHAForms");

            migrationBuilder.DropColumn(
                name: "FormTypeId",
                table: "FLHAForms");

            migrationBuilder.DropColumn(
                name: "OtherComments",
                table: "FLHAForms");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "FLHAForms",
                newName: "ProjectFormId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProjectForms",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FLHAForms",
                table: "FLHAForms",
                column: "ProjectFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_FLHAFormHazards_FLHAForms_FLHAFormId",
                table: "FLHAFormHazards",
                column: "FLHAFormId",
                principalTable: "FLHAForms",
                principalColumn: "ProjectFormId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FLHAForms_ProjectForms_ProjectFormId",
                table: "FLHAForms",
                column: "ProjectFormId",
                principalTable: "ProjectForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FLHAFormHazards_FLHAForms_FLHAFormId",
                table: "FLHAFormHazards");

            migrationBuilder.DropForeignKey(
                name: "FK_FLHAForms_ProjectForms_ProjectFormId",
                table: "FLHAForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FLHAForms",
                table: "FLHAForms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProjectForms");

            migrationBuilder.RenameColumn(
                name: "ProjectFormId",
                table: "FLHAForms",
                newName: "ProjectId");

            migrationBuilder.AddColumn<int>(
                name: "FLHAFormId",
                table: "FormSignatures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FLHAFormId",
                table: "FormPhotos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FLHAFormId",
                table: "FormParticipants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FLHAForms",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "FLHAForms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFilled",
                table: "FLHAForms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FormTypeId",
                table: "FLHAForms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OtherComments",
                table: "FLHAForms",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FLHAForms",
                table: "FLHAForms",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FormSignatures_FLHAFormId",
                table: "FormSignatures",
                column: "FLHAFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormPhotos_FLHAFormId",
                table: "FormPhotos",
                column: "FLHAFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormParticipants_FLHAFormId",
                table: "FormParticipants",
                column: "FLHAFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FLHAForms_CreatorId",
                table: "FLHAForms",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_FLHAFormHazards_FLHAForms_FLHAFormId",
                table: "FLHAFormHazards",
                column: "FLHAFormId",
                principalTable: "FLHAForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FLHAForms_Users_CreatorId",
                table: "FLHAForms",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormParticipants_FLHAForms_FLHAFormId",
                table: "FormParticipants",
                column: "FLHAFormId",
                principalTable: "FLHAForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormPhotos_FLHAForms_FLHAFormId",
                table: "FormPhotos",
                column: "FLHAFormId",
                principalTable: "FLHAForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSignatures_FLHAForms_FLHAFormId",
                table: "FormSignatures",
                column: "FLHAFormId",
                principalTable: "FLHAForms",
                principalColumn: "Id");
        }
    }
}
