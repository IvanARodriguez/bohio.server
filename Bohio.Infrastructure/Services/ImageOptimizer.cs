using SixLabors.ImageSharp.Formats.Webp;
using Bohio.Core.Interfaces;

namespace Bohio.Infrastructure.Services
{
    public class ImageOptimizer : IImageOptimizer
    {
        public async Task<byte[]> OptimizeAsync(Stream imageStream, int width)
        {
            using var image = await Image.LoadAsync(imageStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, 0),
                Mode = ResizeMode.Max
            }));

            using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, new WebpEncoder { Quality = 80 });

            return outputStream.ToArray();
        }
    }
}