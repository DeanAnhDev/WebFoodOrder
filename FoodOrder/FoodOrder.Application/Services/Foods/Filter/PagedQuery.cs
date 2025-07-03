namespace FoodOrder.Application.Services.Foods.Filter
{
    public class PagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortOrder { get; set; } = "desc";
        public string? CategoryName { get; set; }
        public string? Name { get; set; }
        public bool? Status { get; set; }
        public bool? IsOutOfStock { get; set; }
    }
}
