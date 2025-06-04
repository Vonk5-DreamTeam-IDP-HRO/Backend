using AutoMapper;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.Models;

namespace Routeplanner_API.Mappers
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            // Mapping from Location (EF Core entity) to LocationDto
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.LocationDetail, opt => opt.MapFrom(src => src.LocationDetail));

            // Mapping from CreateLocationDto to Location (for creation)
            CreateMap<CreateLocationDto, Location>()
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()) // Ignore ID during creation
                .ForMember(dest => dest.UserId, opt => opt.MapFrom((src, dest, destMember, context) => (Guid?)context.Items["UserId"]))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LocationDetail, opt => opt.MapFrom(src => src.LocationDetail)); // Map nested DTO

            // Mapping from UpdateLocationDto to Location (for update)
            CreateMap<UpdateLocationDto, Location>()
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()) // ID from route, not DTO
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Don't map UserId on update
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Don't update CreatedAt
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set UpdatedAt to now
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Only map non-null values

            // Mapping from LocationDetailDto to LocationDetail (fix missing mapping)
            CreateMap<LocationDetailDto, LocationDetail>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Optional: ignore nulls

            // Mapping from LocationDetail to LocationDetailDto
            CreateMap<LocationDetail, LocationDetailDto>();

            // Mapping from CreateLocationDetailDto to LocationDetail (for creation)
            CreateMap<CreateLocationDetailDto, LocationDetail>()
                .ForMember(dest => dest.LocationDetailsId, opt => opt.Ignore()) // Ignore ID on creation
                .ForMember(dest => dest.LocationId, opt => opt.Ignore()); // Will be set later in the process if needed
        }
    }
}
