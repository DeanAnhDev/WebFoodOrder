namespace FoodOrder.Application.Services.Foods.Filter
{
    public class PagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Ví dụ: "asc" hoặc "desc"
        public string SortOrder { get; set; } = "desc";

        // Tên danh mục để tìm kiếm
        public string? CategoryName { get; set; }
        public string? Name { get; set; }
    }
}
