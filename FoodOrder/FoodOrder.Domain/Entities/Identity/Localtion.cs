

namespace FoodOrder.Domain.Entities.Identity
{
    public class Localtion
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser? Users { get; set; }
        public string Address { get; set; } = null!;
        public string? ShortAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Remarks { get; set; }

        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
