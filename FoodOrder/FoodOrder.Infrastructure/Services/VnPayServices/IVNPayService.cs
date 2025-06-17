

namespace FoodOrder.Infrastructure.Services.VnPayServices
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(decimal amount, string orderId, string orderInfo);
        bool ValidatePayment(string responseData);
    }
}
