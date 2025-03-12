using Microsoft.AspNetCore.Http;

namespace Bohio.Core
{
    public class MediaUploadRequest
    {
        public List<IFormFile> Files { get; set; } = [];
    }
}