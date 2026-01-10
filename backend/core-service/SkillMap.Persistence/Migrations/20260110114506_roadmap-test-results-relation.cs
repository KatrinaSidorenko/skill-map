using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class roadmaptestresultsrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ScoredPoints",
                table: "user_test_results",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "MaxPoints",
                table: "user_test_results",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "user_test_results",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_test_results_user_roadmap_tests_UserRoadmapTestId1",
                table: "user_test_results");

            migrationBuilder.DropIndex(
                name: "IX_user_test_results_UserRoadmapTestId1",
                table: "user_test_results");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "user_test_results");

            migrationBuilder.AlterColumn<int>(
                name: "ScoredPoints",
                table: "user_test_results",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPoints",
                table: "user_test_results",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
