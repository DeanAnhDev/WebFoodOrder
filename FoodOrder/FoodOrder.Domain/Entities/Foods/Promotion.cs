
using System.Text.Json.Serialization;

namespace FoodOrder.Domain.Entities.Foods
{
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public PromotionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Food> Foods { get; set; } = new List<Food>();
        public ICollection<Combo> Combos { get; set; } = new List<Combo>();
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PromotionType
    {
        Amount = 1,
        Percentage = 2
    }

}
