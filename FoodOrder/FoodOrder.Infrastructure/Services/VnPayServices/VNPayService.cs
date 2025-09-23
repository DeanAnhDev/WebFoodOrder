using FoodOrder.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FoodOrder.Infrastructure.Services.VnPayServices
{
    public class VNPayService(IConfiguration configuration) : Application.Interfaces.IVNPayService
    {
        public string CreatePaymentUrl(decimal amount, string orderId, string orderInfo)
        {
            var vnpay = new SortedList<string, string>(new VnPayCompare());
            vnpay.Add("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.Add("vnp_Command", "pay");
            vnpay.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.Add("vnp_CurrCode", "VND");
            vnpay.Add("vnp_IpAddr", "::1");
            vnpay.Add("vnp_Locale", "vn");
            vnpay.Add("vnp_OrderInfo", orderInfo);
            vnpay.Add("vnp_OrderType", "other");
            vnpay.Add("vnp_TmnCode", configuration.GetValue<string>("VnPay:TmnCode") ?? throw new ArgumentNullException("VnPay:TmnCode configuration value is missing."));
            vnpay.Add("vnp_ReturnUrl", configuration.GetValue<string>("VnPay:ReturnUrl") ?? throw new ArgumentNullException("VnPay:ReturnUrl configuration value is missing."));
            vnpay.Add("vnp_TxnRef", orderId);
            vnpay.Add("vnp_Version", "2.1.0");
            var signData = new StringBuilder();
            foreach (var kv in vnpay)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                    signData.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
            signData.Length -= 1;
            var hashSecret = configuration.GetValue<string>("VnPay:HashSecret")
            ?? throw new ArgumentNullException(nameof(configuration), "VnPay:HashSecret configuration value is missing.");
            var hash = HmacSHA512(hashSecret, signData.ToString());
            var queryString = string.Join("&", vnpay.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            queryString += "&vnp_SecureHash=" + hash;
            var url = $"{configuration.GetValue<string>("VnPay:BaseUrl")}?{queryString}";
            return url;
        }

        public bool ValidatePayment(string responseData)
        {
            // Sử dụng VnPayCompare để đảm bảo thứ tự sort giống như khi tạo payment URL
            var vnpay = new SortedList<string, string>(new VnPayCompare());
            var responseParams = responseData.Split('&');
            foreach (var param in responseParams)
            {
                var keyValue = param.Split('=');
                if (keyValue.Length == 2)
                {
                    vnpay.Add(keyValue[0], Uri.UnescapeDataString(keyValue[1]));
                }
            }

            // Lấy secure hash từ response và xóa nó khỏi danh sách để validate
            var secureHash = vnpay.ContainsKey("vnp_SecureHash") ? vnpay["vnp_SecureHash"] : "";
            if (vnpay.ContainsKey("vnp_SecureHash"))
                vnpay.Remove("vnp_SecureHash");

            // Tạo signData giống như cách tạo trong CreatePaymentUrl
            var signData = new StringBuilder();
            foreach (var kv in vnpay)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                    signData.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            // Xóa ký tự & cuối cùng
            if (signData.Length > 0)
                signData.Length -= 1;

            var hashSecret = configuration.GetValue<string>("VnPay:HashSecret")
            ?? throw new ArgumentNullException(nameof(configuration), "VnPay:HashSecret configuration value is missing.");
            var hash = HmacSHA512(hashSecret, signData.ToString());

            return secureHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

    }
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return CompareInfo.GetCompareInfo("en-US").Compare(x ?? string.Empty, y ?? string.Empty, CompareOptions.Ordinal);
        }
    }
}
