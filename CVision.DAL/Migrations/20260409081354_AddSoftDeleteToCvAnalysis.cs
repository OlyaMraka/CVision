using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVision.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToCvAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CVAnalysesRecommendations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CVAnalysesRecommendations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CVAnalyses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CVAnalyses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CVAnalysesRecommendations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CVAnalysesRecommendations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CVAnalyses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CVAnalyses");
        }
    }
}
