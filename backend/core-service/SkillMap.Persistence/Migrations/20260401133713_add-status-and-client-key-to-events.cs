using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addstatusandclientkeytoevents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "event_status",
                table: "workspace_event",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "idempotency_key",
                table: "workspace_event",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_event_roadmap_workspace_id_idempotency_key_event_~",
                table: "workspace_event",
                columns: new[] { "roadmap_workspace_id", "idempotency_key", "event_status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_workspace_event_roadmap_workspace_id_idempotency_key_event_~",
                table: "workspace_event");

            migrationBuilder.DropColumn(
                name: "event_status",
                table: "workspace_event");

            migrationBuilder.DropColumn(
                name: "idempotency_key",
                table: "workspace_event");
        }
    }
}