using Bohio.Core.Entities;

namespace Bohio.Core.Interfaces
{
    public interface IHomeSpaceRepository
    {
        Task<List<HomeSpace>> GetHomeSpaceAndImagesAsync();
    }
}