using AutoMapper;

namespace Bohio.Core.Entities
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
