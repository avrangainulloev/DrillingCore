using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFormsStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormTypeId = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true),
                    GroupName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_FormTypes_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "FormTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    FormTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    CrewName = table.Column<string>(type: "text", nullable: true),
                    UnitNumber = table.Column<string>(type: "text", nullable: true),
                    DateFilled = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OtherComments = table.Column<string>(type: "text", nullable: true),
                    AdditionalData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectForms_FormTypes_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "FormTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectForms_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectForms_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormChecklistResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectFormId = table.Column<int>(type: "integer", nullable: false),
                    ChecklistItemId = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormChecklistResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormChecklistResponses_ChecklistItems_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormChecklistResponses_ProjectForms_ProjectFormId",
                        column: x => x.ProjectFormId,
                        principalTable: "ProjectForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectFormId = table.Column<int>(type: "integer", nullable: false),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false),
                    AttachDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DetachDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Signature = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormParticipants_ProjectForms_ProjectFormId",
                        column: x => x.ProjectFormId,
                        principalTable: "ProjectForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormParticipants_Users_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectFormId = table.Column<int>(type: "integer", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormPhotos_ProjectForms_ProjectFormId",
                        column: x => x.ProjectFormId,
                        principalTable: "ProjectForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_FormTypeId",
                table: "ChecklistItems",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormChecklistResponses_ChecklistItemId",
                table: "FormChecklistResponses",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FormChecklistResponses_ProjectFormId",
                table: "FormChecklistResponses",
                column: "ProjectFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormParticipants_ParticipantId",
                table: "FormParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FormParticipants_ProjectFormId",
                table: "FormParticipants",
                column: "ProjectFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormPhotos_ProjectFormId",
                table: "FormPhotos",
                column: "ProjectFormId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectForms_CreatorId",
                table: "ProjectForms",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectForms_FormTypeId",
                table: "ProjectForms",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectForms_ProjectId",
                table: "ProjectForms",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormChecklistResponses");

            migrationBuilder.DropTable(
                name: "FormParticipants");

            migrationBuilder.DropTable(
                name: "FormPhotos");

            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "ProjectForms");

            migrationBuilder.DropTable(
                name: "FormTypes");
        }
    }
}
