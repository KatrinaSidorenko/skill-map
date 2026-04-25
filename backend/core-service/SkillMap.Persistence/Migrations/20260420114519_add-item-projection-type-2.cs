using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class additemprojectiontype2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roadmap_learning_item_projection_roadmap_workspace_id_is_av~",
                table: "roadmap_learning_item_projection");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "roadmap_learning_item_projection",
                newName: "type");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "roadmap_learning_item_projection",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_projection_roadmap_workspace_id_is_av~",
                table: "roadmap_learning_item_projection",
                columns: new[] { "roadmap_workspace_id", "is_available", "status", "type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roadmap_learning_item_projection_roadmap_workspace_id_is_av~",
                table: "roadmap_learning_item_projection");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "roadmap_learning_item_projection",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "roadmap_learning_item_projection",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_projection_roadmap_workspace_id_is_av~",
                table: "roadmap_learning_item_projection",
                columns: new[] { "roadmap_workspace_id", "is_available", "status" });
        }
    }
}
