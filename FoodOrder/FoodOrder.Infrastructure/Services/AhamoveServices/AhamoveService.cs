using FoodOrder.Application.DTOs.Ahamove;
using FoodOrder.Application.Interfaces;
using FoodOrder.Infrastructure.Services.GoongServices;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;

namespace FoodOrder.Infrastructure.Services.AhamoveServices
{
    public class AhamoveService : IAhamoveService
    {
        private readonly HttpClient _httpClient;
        private readonly AhamoveSettings _settings;
        private readonly GoongSettings _goongSettings;

        public AhamoveService(
            HttpClient httpClient,
            IOptions<AhamoveSettings> settings,
            IOptions<GoongSettings> goongSettings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _goongSettings = goongSettings.Value;
        }

        public async Task<EstimateShippingFeeResponseDto> EstimateShippingFeeAsync(EstimateShippingFeeRequestDto request)
        {
            try
            {
                // Get coordinates for from address (store address from config)
                var fromCoordinate = await GetCoordinateFromAddressAsync(_settings.FromAddress);
                if (fromCoordinate == null)
                {
                    return new EstimateShippingFeeResponseDto
                    {
                        Success = false,
                        Message = "Không thể tìm thấy tọa độ địa chỉ cửa hàng"
                    };
                }

                Console.WriteLine($"From coordinate: {fromCoordinate.Lat}, {fromCoordinate.Lng} - {fromCoordinate.Address}");

                // Get coordinates for to address (customer address)
                var toCoordinate = await GetCoordinateFromAddressAsync(request.ToAddress);
                if (toCoordinate == null)
                {
                    return new EstimateShippingFeeResponseDto
                    {
                        Success = false,
                        Message = "Không thể tìm thấy tọa độ địa chỉ giao hàng"
                    };
                }

                Console.WriteLine($"To coordinate: {toCoordinate.Lat}, {toCoordinate.Lng} - {toCoordinate.Address}");

                // Build Ahamove request
                var ahamoveRequest = new AhamoveEstimateRequestDto
                {
                    Order_time = 0,
                    Path = new[]
                    {
                        new AhamovePathDto
                        {
                            Lat = fromCoordinate.Lat,
                            Lng = fromCoordinate.Lng,
                            Address = fromCoordinate.Address,
                            Short_address = "Điểm lấy hàng",
                            Name = _settings.FromName,
                            Mobile = _settings.FromPhone,
                            Remarks = "Đến nơi lấy hàng đọc mã đơn để nhận hàng"
                        },
                        new AhamovePathDto
                        {
                            Lat = toCoordinate.Lat,
                            Lng = toCoordinate.Lng,
                            Address = toCoordinate.Address,
                            Short_address = "Điểm giao hàng",
                            Name = request.ToName,
                            Mobile = request.ToPhone,
                            Cod = request.CodAmount,
                            Item_value = request.ItemValue,
                            Tracking_number = "TRACK" + DateTime.Now.Ticks.ToString()[..6],
                            Remarks = request.Remarks ?? "Giao hàng nhanh, gọi trước khi đến"
                        }
                    },
                    Services = new[]
                    {
                        new AhamoveServiceDto
                        {
                            _id = "HAN-BIKE",
                            Requests = new[]
                            {
                                new AhamoveServiceRequestDto
                                {
                                    _id = "HAN-BIKE-TIP",
                                    Num = 1
                                }
                            }
                        }
                    },
                    Payment_method = "CASH",
                    Remarks = "Giao hàng nội thành Hà Nội",
                    Promo_code = "AHMKM",
                    Items = new[]
                    {
                        new AhamoveItemDto
                        {
                            _id = "ITM1",
                            Num = 1,
                            Name = "Đồ ăn",
                            Price = request.ItemValue
                        }
                    },
                    Package_detail = new[]
                    {
                        new AhamovePackageDetailDto
                        {
                            Weight = 2,
                            Length = 0.3,
                            Width = 0.3,
                            Height = 0.2,
                            Description = "Đồ ăn thức uống"
                        }
                    }
                };

                // Call Ahamove API
                var json = JsonSerializer.Serialize(ahamoveRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                Console.WriteLine($"Ahamove Request JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.Token}");

                var response = await _httpClient.PostAsync($"{_settings.BaseUrl}/v3/orders/estimates", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ahamove API Error - Status: {response.StatusCode}, Content: {errorContent}");
                    return new EstimateShippingFeeResponseDto
                    {
                        Success = false,
                        Message = $"Lỗi API Ahamove: {response.StatusCode} - {errorContent}"
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // Log the actual response for debugging
                Console.WriteLine($"Ahamove API Response: {responseContent}");

                // Parse JSON response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                // Ahamove returns an array, get the first element
                if (root.ValueKind != JsonValueKind.Array)
                {
                    return new EstimateShippingFeeResponseDto
                    {
                        Success = false,
                        Message = "Response không phải là array như mong đợi"
                    };
                }

                if (root.GetArrayLength() == 0)
                {
                    return new EstimateShippingFeeResponseDto
                    {
                        Success = false,
                        Message = "Response array rỗng"
                    };
                }

                var firstResult = root[0];

                // Check if there's an error in the result
                if (firstResult.TryGetProperty("error", out var errorProp) && errorProp.ValueKind != JsonValueKind.Null)
                {
                    var errorMessage = errorProp.GetString() ?? "Unknown error";
                    return new EstimateShippingFeeResponseDto
                    {
                        Success = false,
                        Message = $"Lỗi từ Ahamove: {errorMessage}"
                    };
                }

                // Get total fee from data.total_fee
                if (firstResult.TryGetProperty("data", out var dataProp) &&
                    dataProp.TryGetProperty("total_fee", out var totalFeeProp))
                {
                    var totalFee = totalFeeProp.GetInt32();

                    return new EstimateShippingFeeResponseDto
                    {
                        Success = true,
                        Message = "Tính phí giao hàng thành công",
                        Fee = totalFee,
                        Currency = "VND"
                    };
                }

                return new EstimateShippingFeeResponseDto
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin phí giao hàng trong response"
                };
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Error: {jsonEx.Message}");
                return new EstimateShippingFeeResponseDto
                {
                    Success = false,
                    Message = $"Lỗi parse JSON: {jsonEx.Message}"
                };
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return new EstimateShippingFeeResponseDto
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {httpEx.Message}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return new EstimateShippingFeeResponseDto
                {
                    Success = false,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        public async Task<LocationCoordinateDto?> GetCoordinateFromAddressAsync(string address)
        {
            try
            {
                var url = $"https://rsapi.goong.io/Geocode?address={Uri.EscapeDataString(address)}&api_key={_goongSettings.ApiKey}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Goong API Error - Status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Goong API Response: {content}");

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // Check if results property exists and is an array
                if (!root.TryGetProperty("results", out var resultsElement))
                {
                    Console.WriteLine("No 'results' property found in Goong response");
                    return null;
                }

                if (resultsElement.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine("'results' is not an array");
                    return null;
                }

                if (resultsElement.GetArrayLength() == 0)
                {
                    Console.WriteLine("Results array is empty");
                    return null;
                }

                var firstResult = resultsElement[0];

                // Check if geometry exists
                if (!firstResult.TryGetProperty("geometry", out var geometry))
                {
                    Console.WriteLine("No 'geometry' property found");
                    return null;
                }

                // Check if location exists
                if (!geometry.TryGetProperty("location", out var location))
                {
                    Console.WriteLine("No 'location' property found in geometry");
                    return null;
                }

                // Get coordinates
                if (!location.TryGetProperty("lat", out var latProp) ||
                    !location.TryGetProperty("lng", out var lngProp))
                {
                    Console.WriteLine("Missing lat/lng properties");
                    return null;
                }

                var formattedAddress = firstResult.TryGetProperty("formatted_address", out var addrProp)
                    ? addrProp.GetString()
                    : address;

                var result = new LocationCoordinateDto
                {
                    Lat = latProp.GetDouble(),
                    Lng = lngProp.GetDouble(),
                    Address = formattedAddress ?? address
                };

                Console.WriteLine($"Parsed coordinates: {result.Lat}, {result.Lng} - {result.Address}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCoordinateFromAddressAsync: {ex.Message}");
                return null;
            }
        }
    }
}