namespace FoodOrder.Application.DTOs.Carts
{
    public class CartResponse
    {
        public CartDto Cart { get; set; } = default!;
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Tổng tiền sau khi đã áp dụng khuyến mãi
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Tổng tiền gốc (trước khi giảm giá)
        /// </summary>
        public decimal OriginalTotal { get; set; }

        /// <summary>
        /// Tổng số tiền đã được giảm
        /// </summary>
        public decimal TotalDiscount { get; set; }
    }
}
