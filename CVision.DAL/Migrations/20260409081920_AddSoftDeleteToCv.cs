using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVision.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CVs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CVs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CVs");
        }
    }
}
