using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateworkspacemetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
                table: "roadmap_workspace");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "roadmap_workspace",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "roadmap_workspace",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "roadmap_workspace",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "roadmap_workspace");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "roadmap_workspace");

            migrationBuilder.DropColumn(
                name: "title",
                table: "roadmap_workspace");

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                table: "roadmap_workspace",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
