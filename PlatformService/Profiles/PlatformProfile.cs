using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<PlatformReadDto, Platform>().ReverseMap();
            CreateMap<PlatformCreateDto, Platform>().ReverseMap();
        }
    }
}
