using AutoMapper;
using Routeplanner_API.DTO;
using Routeplanner_API.Models;

namespace Routeplanner_API.Mappers
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            // TODO: Communicate with team to check if this is the correct mapping

            // Source -> Target
            // Mapping from Location (EF Core entity) to LocationDto
            CreateMap<Location, LocationDto>();

            // Mapping from CreateLocationDto to Location (EF Core entity)
            CreateMap<CreateLocationDto, Location>()
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()) // Don't map ID for creation
                .ForMember(dest => dest.UserId, opt => opt.Ignore())    // Assuming UserId is handled separately or not set directly from this DTO
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping from UpdateLocationDto to Location (EF Core entity)
            CreateMap<UpdateLocationDto, Location>()
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()) // ID is typically from route, not DTO
                .ForMember(dest => dest.UserId, opt => opt.Ignore())    // Assuming UserId is handled separately
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set UpdatedAt on update
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Only map non-null properties for updates
        }
    }
}
