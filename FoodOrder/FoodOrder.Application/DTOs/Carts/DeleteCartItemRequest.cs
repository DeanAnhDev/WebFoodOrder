using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Application.DTOs.Carts
{
    public class DeleteCartItemRequest
    {
        [Required(ErrorMessage = "CartId là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "CartId phải lớn hơn 0")]
        public int CartId { get; set; }

        [Required(ErrorMessage = "CartItemId là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "CartItemId phải lớn hơn 0")]
        public int CartItemId { get; set; }
    }
}