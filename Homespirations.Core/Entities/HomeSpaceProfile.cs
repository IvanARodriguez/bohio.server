using AutoMapper;

namespace Homespirations.Core.Entities;
public class HomeSpaceProfile : Profile
{
    /// <summary>
    /// AutoMapper configuration for HomeSpace.
    /// </summary>
    public HomeSpaceProfile()
    {
        CreateMap<HomeSpace, HomeSpace>();
        CreateMap<HomeSpace, HomeSpacesFeed>();
    }
}
