using AutoMapper;
using Bohio.Infrastructure.Identity;

namespace Bohio.Infrastructure.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<AppUser, User>();
        }
    }
}