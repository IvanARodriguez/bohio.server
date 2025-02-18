using Homespirations.Core.Entities;

namespace Homespirations.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<HomeSpace> HomeSpaces { get; }
        IHomeSpaceRepository HomeSpaceAndMedia { get; }
        IRepository<Media> Media { get; }
        Task<int> SaveChangesAsync();
    }
}
