using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Entities.Image
{
    public class Images
    {
        public string? Id { get; set; }
        public string? Url { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Name { get; set; }
        public int? FoodId { get; set; }
        public Food? Foods { get; set; }
        public int? ComboId { get; set; }
        public Combo? Combos { get; set; }
        public int? CategoryId { get; set; }
        public FoodCategory? FoodCategory { get; set; }
    }
}
