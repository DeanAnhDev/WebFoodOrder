using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "DiscountPercent",
                table: "Promotions",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Foods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsOutOfStock",
                table: "Foods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Combos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsOutOfStock",
                table: "Combos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "IsOutOfStock",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "IsOutOfStock",
                table: "Combos");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercent",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}
