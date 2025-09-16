using FoodOrder.Domain.Entities.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrder.Application.DTOs.Foods.Promotion
{
    public class PromotionUpdateDto
    {
        public string PromotionName { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public PromotionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<int> FoodIds { get; set; } = new();
        public List<int> ComboIds { get; set; } = new();
    }
}
