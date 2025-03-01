using AutoMapper;

namespace Homespirations.Core.Entities
{
    public class MediaProfile : Profile
    {
        public MediaProfile()
        {
            CreateMap<Media, MediaRequest>().ReverseMap();
            CreateMap<MediaRequest, Media>().ReverseMap();
        }

    }
}