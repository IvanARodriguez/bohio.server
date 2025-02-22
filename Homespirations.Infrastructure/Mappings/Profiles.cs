using AutoMapper;
using Homespirations.Infrastructure.Identity;

namespace Homespirations.Infrastructure.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<AppUser, User>();
        }
    }
}