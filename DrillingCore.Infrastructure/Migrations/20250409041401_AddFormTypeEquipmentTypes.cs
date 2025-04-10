using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrillingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFormTypeEquipmentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormTypeEquipmentTypes",
                columns: table => new
                {
                    FormTypeId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTypeEquipmentTypes", x => new { x.FormTypeId, x.EquipmentTypeId });
                    table.ForeignKey(
                        name: "FK_FormTypeEquipmentTypes_EquipmentTypes_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormTypeEquipmentTypes_FormTypes_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "FormTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FormTypeEquipmentTypes",
                columns: new[] { "EquipmentTypeId", "FormTypeId" },
                values: new object[] { 1, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ProjectId",
                table: "Participants",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTypeEquipmentTypes_EquipmentTypeId",
                table: "FormTypeEquipmentTypes",
                column: "EquipmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Projects_ProjectId",
                table: "Participants",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Projects_ProjectId",
                table: "Participants");

            migrationBuilder.DropTable(
                name: "FormTypeEquipmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Participants_ProjectId",
                table: "Participants");
        }
    }
}
