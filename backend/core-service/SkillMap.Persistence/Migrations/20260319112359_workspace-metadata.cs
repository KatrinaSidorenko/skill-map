using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class workspacemetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "metadata",
                table: "roadmap_workspace",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
                table: "roadmap_workspace");
        }
    }
}