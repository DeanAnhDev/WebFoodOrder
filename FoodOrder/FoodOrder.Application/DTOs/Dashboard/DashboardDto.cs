using System;
using System.Collections.Generic;

namespace FoodOrder.Application.DTOs.Dashboard
{
    public class TopProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int SoldQuantity { get; set; }
        public string ProductType { get; set; } = string.Empty; // "Food" hoặc "Combo"
    }

    public class TopProductsResponseDto
    {
        public List<TopProductDto> TopFoods { get; set; } = new();
        public List<TopProductDto> TopCombos { get; set; } = new();
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalFoodsSold { get; set; }
        public int TotalCombosSold { get; set; }
    }

    public class TopProductsRequestDto
    {
        public int Limit { get; set; } = 5;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<TopProductDto> TopProducts { get; set; } = new();
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}