using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CVision.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCvLookupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CvLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LookupWord = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CvId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvLookups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvLookups_CVs_CvId",
                        column: x => x.CvId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvLookups_CvId",
                table: "CvLookups",
                column: "CvId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvLookups");
        }
    }
}
