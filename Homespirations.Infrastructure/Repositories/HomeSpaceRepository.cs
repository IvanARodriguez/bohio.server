using Homespirations.Core.Entities;
using Homespirations.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Homespirations.Infrastructure.Repositories
{
    public class HomeSpaceRepository : IRepository<HomeSpace>
    {
        private readonly DbContext _context;

        public HomeSpaceRepository(DbContext context)
        {
            _context = context;
        }

        public Task AddAsync(HomeSpace entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Ulid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<HomeSpace>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HomeSpace?> GetByIdAsync(Ulid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(HomeSpace entity)
        {
            throw new NotImplementedException();
        }
    }
}