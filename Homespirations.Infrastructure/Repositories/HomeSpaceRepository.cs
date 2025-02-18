using Microsoft.EntityFrameworkCore;
using Homespirations.Core.Entities;
using Homespirations.Core.Interfaces;

namespace Homespirations.Infrastructure.Repositories
{
    public class HomeSpaceRepository(AppDbContext context) : IHomeSpaceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<HomeSpace>> GetHomeSpaceAndImagesAsync()
        {
            var homeSpaces = await _context.HomeSpaces
                .Include(h => h.MediaItems) // Eagerly load related images
                .ToListAsync();

            return homeSpaces;
        }
    }
}
