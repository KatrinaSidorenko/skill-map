using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMap.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addattemptuserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "assessment_attempt",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_assessment_attempt_assessment_id_user_id",
                table: "assessment_attempt",
                columns: new[] { "assessment_id", "user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_assessment_attempt_assessment_id_user_id",
                table: "assessment_attempt");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "assessment_attempt");
        }
    }
}
