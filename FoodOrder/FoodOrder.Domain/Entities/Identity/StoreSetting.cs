namespace FoodOrder.Domain.Entities.Identity
{
    public class StoreSetting
    {
        public int Id { get; set; }
        public DateTime OpenTime { get; set; } = DateTime.Today.AddHours(8); // Mặc định 8:00 sáng
        public DateTime CloseTime { get; set; }
        public bool IsClosedToday { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    }
}
