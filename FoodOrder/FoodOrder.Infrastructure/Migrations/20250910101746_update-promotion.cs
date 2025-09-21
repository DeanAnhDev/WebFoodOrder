using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatepromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Combos_ComboId",
                table: "Promotions");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Foods_FoodId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_ComboId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_FoodId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "FoodId",
                table: "Promotions");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Promotions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Foods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Combos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Foods_PromotionId",
                table: "Foods",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Combos_PromotionId",
                table: "Combos",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Combos_Promotions_PromotionId",
                table: "Combos",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Foods_Promotions_PromotionId",
                table: "Foods",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Combos_Promotions_PromotionId",
                table: "Combos");

            migrationBuilder.DropForeignKey(
                name: "FK_Foods_Promotions_PromotionId",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_Foods_PromotionId",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_Combos_PromotionId",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Combos");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Promotions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "ComboId",
                table: "Promotions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "DiscountPercent",
                table: "Promotions",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoodId",
                table: "Promotions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_ComboId",
                table: "Promotions",
                column: "ComboId",
                unique: true,
                filter: "[ComboId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_FoodId",
                table: "Promotions",
                column: "FoodId",
                unique: true,
                filter: "[FoodId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Combos_ComboId",
                table: "Promotions",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "ComboId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Foods_FoodId",
                table: "Promotions",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "FoodId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
