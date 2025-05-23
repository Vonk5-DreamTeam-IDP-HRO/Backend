using AutoMapper;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.Models;

namespace Routeplanner_API.Mappers
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            // Source -> Target
            // Mapping from Location (EF Core entity) to LocationDto
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.LocationDetail, opt => opt.MapFrom(src => src.LocationDetail));

            // Mapping from CreateLocationDto to Location (EF Core entity)
            CreateMap<CreateLocationDto, Location>()
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()) // Don't map ID for creation
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Map UserId from DTO
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LocationDetail, opt => opt.MapFrom(src => src.LocationDetail)); // Map nested DTO

            // Mapping from UpdateLocationDto to Location (EF Core entity)
            CreateMap<UpdateLocationDto, Location>()
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()) // ID is typically from route, not DTO
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Don't map UserId for update
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Don't change CreatedAt on update
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set UpdatedAt on update
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Only map non-null properties for updates

            // Mapping for LocationDetail
            CreateMap<CreateLocationDetailDto, LocationDetail>()
                .ForMember(dest => dest.LocationDetailsId, opt => opt.Ignore()) // Don't map ID for creation, DB generates it
                .ForMember(dest => dest.LocationId, opt => opt.Ignore());

            CreateMap<LocationDetail, LocationDetailDto>();
        }
    }
}
