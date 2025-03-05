
namespace FoodOrder.Domain.Entities.Foods
{
    public class ComboDetail
    {
        public int ComboId { get; set; }
        public Combo? Combo { get; set; }
        public int FoodId { get; set; }
        public Food? Food { get; set; }
        public int Quantity { get; set; }
    }
}
