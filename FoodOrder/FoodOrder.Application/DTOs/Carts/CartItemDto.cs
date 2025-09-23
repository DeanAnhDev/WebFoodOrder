using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Queries;

namespace FoodOrder.Application.DTOs.Carts
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        //public Cart? Cart { get; set; }
        public int? FoodId { get; set; }
        public FoodDto? Food { get; set; }
        public int? ComboId { get; set; }
        public ComboDto? Combo { get; set; }
        public int Quantity { get; set; }

        /// <summary>
        /// Giá gốc của từng item
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Giá sau khuyến mãi của từng item
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Tổng tiền gốc cho item này (OriginalPrice * Quantity)
        /// </summary>
        public decimal OriginalTotal { get; set; }

        /// <summary>
        /// Tổng tiền sau khuyến mãi cho item này (FinalPrice * Quantity)
        /// </summary>
        public decimal FinalTotal { get; set; }

        /// <summary>
        /// Số tiền được giảm cho item này
        /// </summary>
        public decimal DiscountAmount { get; set; }
    }
}
