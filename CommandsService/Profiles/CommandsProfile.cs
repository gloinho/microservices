﻿using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile() 
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command,CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalId, options => options.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, options => options.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Commands, options => options.Ignore());
        }
    }
}
