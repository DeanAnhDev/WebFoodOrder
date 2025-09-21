namespace FoodOrder.Application.DTOs.Ahamove
{
    // Internal DTOs for Ahamove API
    public class AhamovePathDto
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Short_address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int Cod { get; set; }
        public int Item_value { get; set; }
        public string? Tracking_number { get; set; }
    }

    public class AhamoveServiceRequestDto
    {
        public string _id { get; set; } = string.Empty;
        public int Num { get; set; }
    }

    public class AhamoveServiceDto
    {
        public string _id { get; set; } = string.Empty;
        public AhamoveServiceRequestDto[] Requests { get; set; } = Array.Empty<AhamoveServiceRequestDto>();
    }

    public class AhamoveItemDto
    {
        public string _id { get; set; } = string.Empty;
        public int Num { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
    }

    public class AhamovePackageDetailDto
    {
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class AhamoveEstimateRequestDto
    {
        public int Order_time { get; set; }
        public AhamovePathDto[] Path { get; set; } = Array.Empty<AhamovePathDto>();
        public AhamoveServiceDto[] Services { get; set; } = Array.Empty<AhamoveServiceDto>();
        public string Payment_method { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string? Promo_code { get; set; }
        public AhamoveItemDto[] Items { get; set; } = Array.Empty<AhamoveItemDto>();
        public AhamovePackageDetailDto[] Package_detail { get; set; } = Array.Empty<AhamovePackageDetailDto>();
    }

    public class AhamoveEstimateResponseDto
    {
        public int Total { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}