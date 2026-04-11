using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class learningitemprojectionrename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roadmap_learning_item_status");

            migrationBuilder.CreateTable(
                name: "roadmap_learning_item_projection",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roadmap_workspace_id = table.Column<long>(type: "bigint", nullable: false),
                    learning_item_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_learning_item_projection", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadmap_learning_item_projection_roadmap_workspace_roadmap_~",
                        column: x => x.roadmap_workspace_id,
                        principalTable: "roadmap_workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_projection_roadmap_workspace_id_is_av~",
                table: "roadmap_learning_item_projection",
                columns: new[] { "roadmap_workspace_id", "is_available", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_learning_item_projection_roadmap_workspace_id_learn~",
                table: "roadmap_learning_item_projection",
                columns: new[] { "roadmap_workspace_id", "learning_item_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roadmap_learning_item_projection");

            migrationBuilder.CreateTable(
                name: "roadmap_learning_item_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roadmap_workspace_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    learning_item_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_learning_item_status", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadmap_learning_item_status_roadmap_workspace_roadmap_work~",
                        column: x => x.roadmap_workspace_id,
                        principalTable: "roadmap_workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
    }
}
