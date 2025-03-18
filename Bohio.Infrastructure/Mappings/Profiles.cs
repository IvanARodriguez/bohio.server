using AutoMapper;
using Bohio.Core.DTOs;
using Bohio.Infrastructure.Identity;

namespace Bohio.Infrastructure.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<AppUser, User>();
            CreateMap<AppUser, GetUserResponse>();
        }
    }
}