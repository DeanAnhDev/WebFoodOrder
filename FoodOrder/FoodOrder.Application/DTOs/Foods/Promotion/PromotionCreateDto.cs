using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Application.DTOs.Foods.Promotion
{
    public class PromotionCreateDto
    {
        public string PromotionName { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public PromotionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Danh sách FoodId và ComboId sẽ được gán Promotion khi tạo
        public List<int> FoodIds { get; set; } = new();
        public List<int> ComboIds { get; set; } = new();
    }
}
