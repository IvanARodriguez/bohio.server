namespace Bohio.Core.Interfaces
{
    public interface IImageOptimizer
    {
        Task<byte[]> OptimizeAsync(Stream imageStream, int width);
    }
}