using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Domain.Entities.Foods;


namespace FoodOrder.Application.DTOs.Foods.Promotion
{
    public class PromotionDto
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public PromotionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<FoodDto> Foods { get; set; } = new();
        public List<ComboDto> Combos { get; set; } = new();
    }

}
