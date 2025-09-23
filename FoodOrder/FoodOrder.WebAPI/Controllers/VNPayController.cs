using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;
        private readonly IOrderService _orderService;

        public VNPayController(IVNPayService vnPayService, IOrderService orderService)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
        }

        private static readonly Dictionary<string, DateTime> _paymentTokens = new();

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] PaymentRequest request)
        {
            var paymentUrl = _vnPayService.CreatePaymentUrl(
                amount: request.Amount,
                orderId: request.OrderId,
                orderInfo: request.OrderInfo
            );
            return Ok(new { paymentUrl });
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback([FromQuery] VNPayCallback callback)
        {
            var responseData = Request.QueryString.ToString().TrimStart('?');

            //// Validate payment với VNPay
            //if (!_vnPayService.ValidatePayment(responseData))
            //{
            //    // Thanh toán không hợp lệ - rollback và chuyển về trang thất bại
            //    if (!string.IsNullOrEmpty(callback.vnp_TxnRef))
            //    {
            //        await _orderService.ProcessPaymentFailureAsync(callback.vnp_TxnRef);
            //    }

            //    var failedToken = GenerateToken();
            //    _paymentTokens[failedToken] = DateTime.UtcNow.AddMinutes(5);
            //    return Redirect($"http://localhost:5173/checkout-failed?token={failedToken}");
            //}

            // Kiểm tra response code và transaction status
            if (callback.vnp_ResponseCode == "00" && callback.vnp_TransactionStatus == "00")
            {
                // Thanh toán thành công - xóa cart và cập nhật order
                var success = await _orderService.ProcessPaymentSuccessAsync(callback.vnp_TxnRef);

                if (success)
                {
                    var token = GenerateToken();
                    _paymentTokens[token] = DateTime.UtcNow.AddMinutes(5);
                    return Redirect($"http://localhost:3000/checkout-success?token={token}");
                }
                else
                {
                    // Lỗi khi xử lý thành công - fallback về thất bại
                    await _orderService.ProcessPaymentFailureAsync(callback.vnp_TxnRef);
                    var failedToken = GenerateToken();
                    _paymentTokens[failedToken] = DateTime.UtcNow.AddMinutes(5);
                    return Redirect($"http://localhost:3000/checkout-failed?token={failedToken}");
                }
            }
            else
            {
                // Thanh toán thất bại theo VNPay response - rollback
                await _orderService.ProcessPaymentFailureAsync(callback.vnp_TxnRef);
                var failedToken = GenerateToken();
                _paymentTokens[failedToken] = DateTime.UtcNow.AddMinutes(5);
                return Redirect($"http://localhost:3000/checkout-failed?token={failedToken}");
            }
        }

        [HttpGet("verify-payment-token")]
        public IActionResult VerifyPaymentToken([FromQuery] string token)
        {
            if (_paymentTokens.TryGetValue(token, out var expiryTime))
            {
                if (DateTime.UtcNow <= expiryTime)
                {
                    return Ok(new { isValid = true });
                }
                _paymentTokens.Remove(token);
            }
            return Ok(new { isValid = false });
        }

        [HttpPost("invalidate-payment-token")]
        public IActionResult InvalidatePaymentToken([FromQuery] string token)
        {
            if (_paymentTokens.ContainsKey(token))
            {
                _paymentTokens.Remove(token);
                return Ok(new { success = true });
            }
            return Ok(new { success = false });
        }

        private string GenerateToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }

    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
    }

    public class VNPayCallback
    {
        public string vnp_Amount { get; set; } = string.Empty;
        public string vnp_BankCode { get; set; } = string.Empty;
        public string vnp_BankTranNo { get; set; } = string.Empty;
        public string vnp_CardType { get; set; } = string.Empty;
        public string vnp_OrderInfo { get; set; } = string.Empty;
        public string vnp_PayDate { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_TmnCode { get; set; } = string.Empty;
        public string vnp_TransactionNo { get; set; } = string.Empty;
        public string vnp_TransactionStatus { get; set; } = string.Empty;
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string vnp_SecureHash { get; set; } = string.Empty;
    }
}
