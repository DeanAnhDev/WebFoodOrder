using FoodOrder.Application.DTOs.Ahamove;

namespace FoodOrder.Application.Interfaces
{
    public interface IAhamoveService
    {
        Task<EstimateShippingFeeResponseDto> EstimateShippingFeeAsync(EstimateShippingFeeRequestDto request);
        Task<LocationCoordinateDto?> GetCoordinateFromAddressAsync(string address);
    }
}