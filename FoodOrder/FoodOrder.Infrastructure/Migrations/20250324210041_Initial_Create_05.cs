using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "FoodCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Combos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "FoodCategories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Combos");
        }
    }
}
