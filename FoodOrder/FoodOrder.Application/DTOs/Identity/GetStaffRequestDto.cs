namespace FoodOrder.Application.DTOs.Identity
{
    public class GetStaffRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } // Tìm kiếm theo email, số điện thoại hoặc tên
        public string? Email { get; set; } // Tìm kiếm cụ thể theo email
        public string? PhoneNumber { get; set; } // Tìm kiếm cụ thể theo số điện thoại
    }
}