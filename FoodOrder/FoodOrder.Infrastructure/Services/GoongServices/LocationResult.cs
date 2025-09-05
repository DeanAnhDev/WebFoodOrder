

namespace FoodOrder.Infrastructure.Services.GoongServices
{
    public class LocationResult
    {
        public bool IsInInnerCity { get; set; }
        public string Address { get; set; }

        public LocationResult(bool isInInnerCity, string address)
        {
            IsInInnerCity = isInInnerCity;
            Address = address;
        }
    }

}
