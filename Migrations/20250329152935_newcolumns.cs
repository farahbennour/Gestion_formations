using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Formations.Migrations
{
    /// <inheritdoc />
    public partial class newcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lieu",
                table: "Formations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Prix",
                table: "Formations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lieu",
                table: "Formations");

            migrationBuilder.DropColumn(
                name: "Prix",
                table: "Formations");
        }
    }
}
