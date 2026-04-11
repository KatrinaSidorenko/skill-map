using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class learningitemprojectionfixindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roadmap_learning_item_status_roadmap_workspace_id_is_availa~",
                table: "roadmap_learning_item_status");

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_status_roadmap_workspace_id_is_availa~",
                table: "roadmap_learning_item_status",
                columns: new[] { "roadmap_workspace_id", "is_available", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_status_roadmap_workspace_id_learning_~",
                table: "roadmap_learning_item_status",
                columns: new[] { "roadmap_workspace_id", "learning_item_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roadmap_learning_item_status_roadmap_workspace_id_is_availa~",
                table: "roadmap_learning_item_status");

            migrationBuilder.DropIndex(
                name: "IX_roadmap_learning_item_status_roadmap_workspace_id_learning_~",
                table: "roadmap_learning_item_status");

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_status_roadmap_workspace_id_is_availa~",
                table: "roadmap_learning_item_status",
                columns: new[] { "roadmap_workspace_id", "is_available", "status" },
                unique: true);
        }
    }
}
