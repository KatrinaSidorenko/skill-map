using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SkillMap.Core.Entities.UserRoadmapTest;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class adduserroadmaptests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_roadmap_tests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserRoadmapId = table.Column<long>(type: "bigint", nullable: false),
                    TestType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TestData = table.Column<RoadmapTest>(type: "jsonb", nullable: true),
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
                    ResultData = table.Column<RoadmapTestResult>(type: "jsonb", nullable: true),
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
                name: "IX_user_roadmap_tests_UserRoadmapId_TestType",
                table: "user_roadmap_tests",
                columns: new[] { "UserRoadmapId", "TestType" });

            migrationBuilder.CreateIndex(
                name: "IX_user_test_results_UserRoadmapTestId_CompletedAt",
                table: "user_test_results",
                columns: new[] { "UserRoadmapTestId", "CompletedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_test_results");

            migrationBuilder.DropTable(
                name: "user_roadmap_tests");
        }
    }
}
