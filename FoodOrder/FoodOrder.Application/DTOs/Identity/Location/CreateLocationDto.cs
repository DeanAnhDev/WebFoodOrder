

using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Application.DTOs.Identity.Location
{
    public class CreateLocationDto
    {
        public int UserId { get; set; }
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
