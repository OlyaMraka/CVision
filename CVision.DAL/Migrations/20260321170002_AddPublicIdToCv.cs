using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVision.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "CVs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "CVs");
        }
    }
}
