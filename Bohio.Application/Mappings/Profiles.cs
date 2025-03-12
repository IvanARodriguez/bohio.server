using AutoMapper;
using Bohio.Core.Results;
using Microsoft.AspNetCore.Identity;

namespace Bohio.Application.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<IdentityError, Error>()
            .ConstructUsing(src => new Error(src.Code, src.Description))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code));
        }
    }
}