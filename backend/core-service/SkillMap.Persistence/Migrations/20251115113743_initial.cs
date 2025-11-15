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
                name: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_roadmaps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoadmapId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsOwner = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roadmaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roadmaps_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_modifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRoadmapId = table.Column<long>(type: "bigint", nullable: false),
                    InnerItemId = table.Column<string>(type: "text", nullable: true),
                    ExternalItemId = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_modifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roadmap_modifications_user_roadmaps_UserRoadmapId",
                        column: x => x.UserRoadmapId,
                        principalTable: "user_roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_snapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRoadmapId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roadmap_snapshots_user_roadmaps_UserRoadmapId",
                        column: x => x.UserRoadmapId,
                        principalTable: "user_roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roadmap_tests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRoadmapId = table.Column<long>(type: "bigint", nullable: false),
                    TestType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TestData = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roadmap_tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roadmap_tests_user_roadmaps_UserRoadmapId",
                        column: x => x.UserRoadmapId,
                        principalTable: "user_roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_test_results",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRoadmapTestId = table.Column<long>(type: "bigint", nullable: false),
                    MaxPoints = table.Column<int>(type: "integer", nullable: false),
                    ScoredPoints = table.Column<int>(type: "integer", nullable: false),
                    ResultData = table.Column<byte[]>(type: "bytea", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_test_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_test_results_user_roadmap_tests_UserRoadmapTestId",
                        column: x => x.UserRoadmapTestId,
                        principalTable: "user_roadmap_tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_modifications_UserRoadmapId_InnerItemId_ExternalIte~",
                table: "roadmap_modifications",
                columns: new[] { "UserRoadmapId", "InnerItemId", "ExternalItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_roadmap_snapshots_UserRoadmapId_CreatedAt",
                table: "roadmap_snapshots",
                columns: new[] { "UserRoadmapId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roadmap_tests_UserRoadmapId_TestType",
                table: "user_roadmap_tests",
                columns: new[] { "UserRoadmapId", "TestType" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roadmaps_UserId_RoadmapId",
                table: "user_roadmaps",
                columns: new[] { "UserId", "RoadmapId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_test_results_UserRoadmapTestId_CompletedAt",
                table: "user_test_results",
                columns: new[] { "UserRoadmapTestId", "CompletedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roadmap_modifications");

            migrationBuilder.DropTable(
                name: "roadmap_snapshots");

            migrationBuilder.DropTable(
                name: "user_test_results");

            migrationBuilder.DropTable(
                name: "user_roadmap_tests");

            migrationBuilder.DropTable(
                name: "user_roadmaps");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
