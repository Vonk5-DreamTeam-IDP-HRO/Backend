using AutoMapper;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Models;
using System;

namespace Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Map User -> UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash));

        // Map UserDto -> User
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Locations, opt => opt.Ignore())
            .ForMember(dest => dest.Routes, opt => opt.Ignore())
            .ForMember(dest => dest.Right, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        // Map CreateUserDto -> User
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            //.ForMember(dest => dest.UserRightId, opt => opt.MapFrom(src => src.UserRightId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Locations, opt => opt.Ignore())
            .ForMember(dest => dest.Routes, opt => opt.Ignore())
            .ForMember(dest => dest.Right, opt => opt.Ignore());

        // Map UpdateUserDto -> User
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Locations, opt => opt.Ignore())
            .ForMember(dest => dest.Routes, opt => opt.Ignore())
                .ForMember(dest => dest.Right, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    
            CreateMap<UpdateUserDto, User>()
                .AfterMap((src, dest) =>
                {
                    if (src.Email != null)
                    {
                        dest.Email = src.Email;
                    }

                    // Handle User PasswordHash update
                    if (src.PasswordHash != null)
                    {
                        dest.PasswordHash = src.PasswordHash;
                    }
                });
    }
}
