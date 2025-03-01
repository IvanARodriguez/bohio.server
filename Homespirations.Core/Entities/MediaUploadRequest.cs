using Microsoft.AspNetCore.Http;

namespace Homespirations.Core
{
    public class MediaUploadRequest
    {
        public List<IFormFile> Files { get; set; } = [];
    }
}