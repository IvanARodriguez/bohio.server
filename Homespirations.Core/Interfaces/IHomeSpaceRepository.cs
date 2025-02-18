using Homespirations.Core.Entities;

namespace Homespirations.Core.Interfaces
{
    public interface IHomeSpaceRepository
    {
        Task<List<HomeSpace>> GetHomeSpaceAndImagesAsync();
    }
}