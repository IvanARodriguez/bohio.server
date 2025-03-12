namespace Bohio.Core.Interfaces
{
    public interface ICloudStorage
    {
        Task<string> UploadAsync(byte[] imageData, string fileName, string contentType);
    }
}