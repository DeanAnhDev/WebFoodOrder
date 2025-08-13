
namespace FoodOrder.Domain.Entities.Foods
{
    public class Promotion
    {
        public int PromotionId { get; set; }

        public string PromotionName { get; set; } = string.Empty;
        public float? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? FoodId { get; set; }
        public Food? Food { get; set; }

        public int? ComboId { get; set; }
        public Combo? Combo { get; set; }
    }
}
