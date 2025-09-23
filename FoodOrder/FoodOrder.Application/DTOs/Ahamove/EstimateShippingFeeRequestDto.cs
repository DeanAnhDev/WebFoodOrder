using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Application.DTOs.Ahamove
{
    public class EstimateShippingFeeRequestDto
    {
        [Required]
        public string ToAddress { get; set; } = string.Empty;

        [Required]
        public string ToName { get; set; } = string.Empty;

        [Required]
        public string ToPhone { get; set; } = string.Empty;

        public int CodAmount { get; set; } = 0;

        public int ItemValue { get; set; } = 0;

        public string? Remarks { get; set; }
    }
}