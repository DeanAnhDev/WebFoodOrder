using CloudinaryDotNet;

namespace FoodOrder.Infrastructure.Services.CloudinaryServices
{
    public class CloudinarySetting
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        public Cloudinary GetCloudinary()
        {
            var account = new Account(CloudName, ApiKey, ApiSecret);
            return new Cloudinary(account);
        }
    }
}
