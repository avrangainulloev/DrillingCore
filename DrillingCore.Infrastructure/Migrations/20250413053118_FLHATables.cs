using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FLHATables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "FLHAForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    DateFilled = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TaskDescription = table.Column<string>(type: "text", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLHAForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FLHAHazardGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLHAHazardGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FLHAHazards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Label = table.Column<string>(type: "text", nullable: true),
                    ControlSuggestion = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLHAHazards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FLHAHazards_FLHAHazardGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "FLHAHazardGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FLHAFormHazards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FLHAFormId = table.Column<int>(type: "integer", nullable: false),
                    HazardText = table.Column<string>(type: "text", nullable: true),
                    ControlMeasures = table.Column<string>(type: "text", nullable: true),
                    HazardTemplateId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLHAFormHazards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FLHAFormHazards_FLHAForms_FLHAFormId",
                        column: x => x.FLHAFormId,
                        principalTable: "FLHAForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FLHAFormHazards_FLHAHazards_HazardTemplateId",
                        column: x => x.HazardTemplateId,
                        principalTable: "FLHAHazards",
                        principalColumn: "Id");
                });

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
                name: "IX_FLHAFormHazards_FLHAFormId",
                table: "FLHAFormHazards",
                column: "FLHAFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FLHAFormHazards_HazardTemplateId",
                table: "FLHAFormHazards",
                column: "HazardTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FLHAHazards_GroupId",
                table: "FLHAHazards",
                column: "GroupId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormParticipants_FLHAForms_FLHAFormId",
                table: "FormParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_FormPhotos_FLHAForms_FLHAFormId",
                table: "FormPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_FormSignatures_FLHAForms_FLHAFormId",
                table: "FormSignatures");

            migrationBuilder.DropTable(
                name: "FLHAFormHazards");

            migrationBuilder.DropTable(
                name: "FLHAForms");

            migrationBuilder.DropTable(
                name: "FLHAHazards");

            migrationBuilder.DropTable(
                name: "FLHAHazardGroups");

            migrationBuilder.DropIndex(
                name: "IX_FormSignatures_FLHAFormId",
                table: "FormSignatures");

            migrationBuilder.DropIndex(
                name: "IX_FormPhotos_FLHAFormId",
                table: "FormPhotos");

            migrationBuilder.DropIndex(
                name: "IX_FormParticipants_FLHAFormId",
                table: "FormParticipants");

            migrationBuilder.DropColumn(
                name: "FLHAFormId",
                table: "FormSignatures");

            migrationBuilder.DropColumn(
                name: "FLHAFormId",
                table: "FormPhotos");

            migrationBuilder.DropColumn(
                name: "FLHAFormId",
                table: "FormParticipants");
        }
    }
}
