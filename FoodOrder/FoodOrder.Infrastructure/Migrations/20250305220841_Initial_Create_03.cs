using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FoodCategoryId",
                table: "Combos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Combos_FoodCategoryId",
                table: "Combos",
                column: "FoodCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Combos_FoodCategories_FoodCategoryId",
                table: "Combos",
                column: "FoodCategoryId",
                principalTable: "FoodCategories",
                principalColumn: "FoodCategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Combos_FoodCategories_FoodCategoryId",
                table: "Combos");

            migrationBuilder.DropIndex(
                name: "IX_Combos_FoodCategoryId",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "FoodCategoryId",
                table: "Combos");
        }
    }
}
