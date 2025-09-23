namespace FoodOrder.Application.DTOs.Ahamove
{
    public class LocationCoordinateDto
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}