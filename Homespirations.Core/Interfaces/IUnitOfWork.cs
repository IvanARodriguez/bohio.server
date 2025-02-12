using Homespirations.Core.Entities;

namespace Homespirations.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<HomeSpace> HomeSpaces { get; }
        Task<int> SaveChangesAsync();
    }
}