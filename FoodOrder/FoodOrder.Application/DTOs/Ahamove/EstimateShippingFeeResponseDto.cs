namespace FoodOrder.Application.DTOs.Ahamove
{
    public class EstimateShippingFeeResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Fee { get; set; }
        public string Currency { get; set; } = "VND";
    }
}