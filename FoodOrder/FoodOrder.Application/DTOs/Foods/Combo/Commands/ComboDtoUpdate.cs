using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.Combo.Commands
{
    public class ComboDtoUpdate
    {
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public int FoodCategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int Quantity { get; set; }
        public ImageDto? Images { get; set; }
        public List<FoodInComboDto> Foods { get; set; } = new();
    }
}
