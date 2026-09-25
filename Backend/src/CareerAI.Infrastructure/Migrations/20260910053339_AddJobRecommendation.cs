using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobRecommendation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "JobRecommendations",
                newName: "MissingSkills");

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "JobRecommendations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchedSkills",
                table: "JobRecommendations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "JobRecommendations");

            migrationBuilder.DropColumn(
                name: "MatchedSkills",
                table: "JobRecommendations");

            migrationBuilder.RenameColumn(
                name: "MissingSkills",
                table: "JobRecommendations",
                newName: "Reason");
        }
    }
}
