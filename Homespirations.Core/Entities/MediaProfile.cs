using AutoMapper;

namespace Homespirations.Core.Entities
{
    public class MediaProfile : Profile
    {
        public MediaProfile()
        {
            CreateMap<Media, MediaDto>().ReverseMap();
            CreateMap<MediaDto, Media>().ReverseMap();
        }

    }
}