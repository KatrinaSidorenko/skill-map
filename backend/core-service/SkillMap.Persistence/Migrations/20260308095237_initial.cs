using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "personal_roadmap",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    WorkspaceId = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal_roadmap", x => x.id);
                    table.ForeignKey(
                        name: "FK_personal_roadmap_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_workspace",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    roadmap_id = table.Column<string>(type: "text", nullable: false),
                    personal_roadmap_id = table.Column<long>(type: "bigint", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_in_author_mode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_workspace", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadmap_workspace_personal_roadmap_personal_roadmap_id",
                        column: x => x.personal_roadmap_id,
                        principalTable: "personal_roadmap",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roadmap_workspace_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_assessment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roadmap_fork_id = table.Column<long>(type: "bigint", nullable: false),
                    test_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    test_data = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_assessment", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadmap_assessment_roadmap_workspace_roadmap_fork_id",
                        column: x => x.roadmap_fork_id,
                        principalTable: "roadmap_workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_event",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roadmap_fork_id = table.Column<long>(type: "bigint", nullable: false),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    metadata = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_event", x => x.id);
                    table.ForeignKey(
                        name: "FK_workspace_event_roadmap_workspace_roadmap_fork_id",
                        column: x => x.roadmap_fork_id,
                        principalTable: "roadmap_workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_snapshot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roadmap_fork_id = table.Column<long>(type: "bigint", nullable: false),
                    content = table.Column<byte[]>(type: "bytea", nullable: true),
                    latest_version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_snapshot", x => x.id);
                    table.ForeignKey(
                        name: "FK_workspace_snapshot_roadmap_workspace_roadmap_fork_id",
                        column: x => x.roadmap_fork_id,
                        principalTable: "roadmap_workspace",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assessment_attempt",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    assessment_id = table.Column<long>(type: "bigint", nullable: false),
                    max_points = table.Column<double>(type: "double precision", nullable: false),
                    scored_points = table.Column<double>(type: "double precision", nullable: false),
                    result_data = table.Column<byte[]>(type: "bytea", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assessment_attempt", x => x.id);
                    table.ForeignKey(
                        name: "FK_assessment_attempt_roadmap_assessment_assessment_id",
                        column: x => x.assessment_id,
                        principalTable: "roadmap_assessment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assessment_attempt_assessment_id_completed_at",
                table: "assessment_attempt",
                columns: new[] { "assessment_id", "completed_at" });

            migrationBuilder.CreateIndex(
                name: "IX_personal_roadmap_author_id",
                table: "personal_roadmap",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_assessment_roadmap_fork_id_test_type",
                table: "roadmap_assessment",
                columns: new[] { "roadmap_fork_id", "test_type" });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_workspace_author_id_roadmap_id",
                table: "roadmap_workspace",
                columns: new[] { "author_id", "roadmap_id" });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_workspace_personal_roadmap_id",
                table: "roadmap_workspace",
                column: "personal_roadmap_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_event_roadmap_fork_id_event_type",
                table: "workspace_event",
                columns: new[] { "roadmap_fork_id", "event_type" });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_event_roadmap_fork_id_version",
                table: "workspace_event",
                columns: new[] { "roadmap_fork_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_snapshot_roadmap_fork_id_created_at",
                table: "workspace_snapshot",
                columns: new[] { "roadmap_fork_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assessment_attempt");

            migrationBuilder.DropTable(
                name: "workspace_event");

            migrationBuilder.DropTable(
                name: "workspace_snapshot");

            migrationBuilder.DropTable(
                name: "roadmap_assessment");

            migrationBuilder.DropTable(
                name: "roadmap_workspace");

            migrationBuilder.DropTable(
                name: "personal_roadmap");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
