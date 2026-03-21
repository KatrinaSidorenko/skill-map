using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class workspaceunindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_roadmap_workspace_author_id_roadmap_id_personal_roadmap_id",
                table: "roadmap_workspace",
                columns: new[] { "author_id", "roadmap_id", "personal_roadmap_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roadmap_workspace_author_id_roadmap_id_personal_roadmap_id",
                table: "roadmap_workspace");
        }
    }
}
