using Microsoft.AspNetCore.Http;

namespace FoodOrder.Infrastructure.Services.CloudinaryServices
{
    public interface ICloudinaryService
    {
        Task<bool> DeleteFileAsync(string publicId);
        Task<CloudinaryResult> UploadFileAsync(IFormFile file, string? existingPublicId = null);
        Task<List<CloudinaryResult>> UploadBatchAsync(List<IFormFile> files);
    }
}
