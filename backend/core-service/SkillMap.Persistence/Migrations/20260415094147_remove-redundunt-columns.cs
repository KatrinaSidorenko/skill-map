using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removeredunduntcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_workspace_event_roadmap_workspace_id_idempotency_key_event_~",
                table: "workspace_event");

            migrationBuilder.DropColumn(
                name: "metadata",
                table: "workspace_snapshot");

            migrationBuilder.DropColumn(
                name: "event_status",
                table: "workspace_event");

            migrationBuilder.DropColumn(
                name: "rejection_reason",
                table: "workspace_event");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_event_roadmap_workspace_id_idempotency_key",
                table: "workspace_event",
                columns: new[] { "roadmap_workspace_id", "idempotency_key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_workspace_event_roadmap_workspace_id_idempotency_key",
                table: "workspace_event");

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                table: "workspace_snapshot",
                type: "jsonb",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "event_status",
                table: "workspace_event",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "rejection_reason",
                table: "workspace_event",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_event_roadmap_workspace_id_idempotency_key_event_~",
                table: "workspace_event",
                columns: new[] { "roadmap_workspace_id", "idempotency_key", "event_status" });
        }
    }
}
