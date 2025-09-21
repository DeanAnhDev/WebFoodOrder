using Microsoft.Extensions.Options;
using System.Text.Json;


namespace FoodOrder.Infrastructure.Services.GoongServices
{
  

    public class GoongService : IGoongService
    {
        private readonly HttpClient _httpClient;
        private readonly GoongSettings _settings;

        public GoongService(HttpClient httpClient, IOptions<GoongSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<LocationResult?> CheckHanoiAsync(string lat, string lng)
        {
            var url = $"https://rsapi.goong.io/Geocode?latlng={lat},{lng}&api_key={_settings.ApiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Goong API error: {response.StatusCode} - {error}");
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var results = doc.RootElement.GetProperty("results");

            if (results.GetArrayLength() == 0)
                return new LocationResult(false, "Không tìm thấy địa chỉ");

            string? formattedAddress = results[0].GetProperty("formatted_address").GetString();

            if (string.IsNullOrEmpty(formattedAddress))
                return new LocationResult(false, "Không tìm thấy địa chỉ");

            // Check Hà Nội
            if (!formattedAddress.Contains("Hà Nội"))
                return new LocationResult(false, formattedAddress);

            // Danh sách quận nội thành
            string[] innerDistricts =
            {
            "Ba Đình", "Hoàn Kiếm", "Đống Đa", "Hai Bà Trưng",
            "Thanh Xuân", "Cầu Giấy", "Tây Hồ", "Hoàng Mai",
            "Long Biên", "Nam Từ Liêm", "Bắc Từ Liêm", "Hà Đông"
        };

            foreach (var district in innerDistricts)
            {
                if (formattedAddress.Contains(district))
                    return new LocationResult(true, formattedAddress);
            }

            return new LocationResult(false, formattedAddress);
        }
    }

}
