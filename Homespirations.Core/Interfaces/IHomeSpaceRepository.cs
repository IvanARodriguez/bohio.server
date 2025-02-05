using Homespirations.Core.Entities;
using NUlid;

namespace Homespirations.Core.Interfaces;

public interface IHomeSpaceRepository
{
  Task<IEnumerable<HomeSpace>> GetAllAsync();
  Task<HomeSpace?> GetByIdAsync(Ulid id);
  Task AddAsync(HomeSpace homeSpace);
  Task DeleteAsync(Ulid id);
}