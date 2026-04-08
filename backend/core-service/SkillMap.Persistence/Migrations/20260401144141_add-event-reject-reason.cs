using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addeventrejectreason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rejection_reason",
                table: "workspace_event",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rejection_reason",
                table: "workspace_event");
        }
    }
}