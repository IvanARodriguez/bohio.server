using Microsoft.AspNetCore.Http;

namespace Homespirations.Core
{
    public class MediaUploadRequestDTO
    {
        public List<IFormFile> Files { get; set; } = [];
    }
}