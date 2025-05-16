using AutoMapper;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Models;
using System;

namespace Routeplanner_API.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Map from User entity to UserDto
            // Retrieves Email and PasswordHash from the related UserConfidential entity.
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                    src.UserConfidential != null ? src.UserConfidential.Email : null))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));

            // Map from UserDto to User entity
            // Primarily for updating UserConfidential data based on UserDto.
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.UserConfidential, opt => opt.Ignore()) // Handled in AfterMap
                .AfterMap((src, dest) =>
                {
                    if (dest.UserConfidential == null)
                    {
                        // Assumes dest.UserId is already populated on the User entity being mapped to.
                        dest.UserConfidential = new UserConfidential { UserId = dest.UserId };
                    }
                    // UserDto is expected to provide Email and PasswordHash.
                    dest.UserConfidential.Email = src.Email;
                })
                // Ignore other User properties not sourced from UserDto
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Locations, opt => opt.Ignore())
                .ForMember(dest => dest.Routes, opt => opt.Ignore())
                .ForMember(dest => dest.Right, opt => opt.Ignore());

            // Map from CreateUserDto to User entity
            // Creates a new User and its associated UserConfidential entity.
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId is GUID database-generated
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash)) // Map directly to User.PasswordHash
                .ForMember(dest => dest.UserRightId, opt => opt.MapFrom(src => src.UserRightId)) // Map from DTO's UserRightId
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UserConfidential, opt => opt.MapFrom(src => new UserConfidential
                {
                    // UserId for UserConfidential will be set by EF Core relationship fix-up
                    Email = src.Email
                    // PasswordHash is on User entity, not UserConfidential
                }))
                // Ignore navigation properties; they are typically handled separately after creation.
                .ForMember(dest => dest.Locations, opt => opt.Ignore())
                .ForMember(dest => dest.Routes, opt => opt.Ignore())
                .ForMember(dest => dest.Right, opt => opt.Ignore()); // Ignores the navigation property User.Right, RightId FK is mapped above

            // Map from UpdateUserDto to User entity
            // For partial updates of a User and its UserConfidential data.
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.UserConfidential, opt => opt.Ignore()) // Handled in AfterMap
                                                                               // Ignore properties not typically updated or managed by this DTO.
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // obviously ignored because only update and must not be updated.
                .ForMember(dest => dest.Locations, opt => opt.Ignore())
                .ForMember(dest => dest.Routes, opt => opt.Ignore())
                .ForMember(dest => dest.Right, opt => opt.Ignore())
                // Apply mapping for top-level properties only if the source member is not null.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // The AfterMap method is used to handle the mapping of UserConfidential properties
            // that are not directly part of the User entity.
            // This is necessary because UserConfidential is a separate entity and not all properties
            // are directly mapped from the User entity.
            // AfterMap to handle User(Confidential) updates conditionally for UpdateUserDto.
            // This ensures that Email and PasswordHash are only updated if provided in the DTO.
            CreateMap<UpdateUserDto, User>()

                .AfterMap((src, dest) =>
                {
                    // Handle UserConfidential Email update
                    if (src.Email != null)
                    {
                        if (dest.UserConfidential == null)
                        {
                            dest.UserConfidential = new UserConfidential { UserId = dest.UserId };
                        }
                        dest.UserConfidential.Email = src.Email;
                    }

                    // Handle User PasswordHash update
                    if (src.PasswordHash != null)
                    {
                        dest.PasswordHash = src.PasswordHash; // PasswordHash is directly on the User entity
                    }
                });
        }
    }
}
