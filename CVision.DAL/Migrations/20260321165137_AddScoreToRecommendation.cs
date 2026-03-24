using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVision.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreToRecommendation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionScore",
                table: "CVAnalysesRecommendations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionScore",
                table: "CVAnalysesRecommendations");
        }
    }
}
