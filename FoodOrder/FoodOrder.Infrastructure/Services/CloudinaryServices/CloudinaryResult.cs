

namespace FoodOrder.Infrastructure.Services.CloudinaryServices
{
    public class CloudinaryResult
    {

        public CloudinaryResult(string url, string id, string thumbnailUrl, string name)
        {
            Url = url;
            Id = id;
            ThumbnailUrl = thumbnailUrl;
            Name = name;
        }

        public string Url { get; set; }
        public string Id { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Name { get; set; }
    }
}
