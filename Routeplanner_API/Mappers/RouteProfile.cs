using AutoMapper;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.Models;
using System;
using Route = Routeplanner_API.Models.Route;

namespace Routeplanner_API.Mappers
{
    public class RouteProfile : Profile
    {
        public RouteProfile()
        {
            // Mapping from Route (EF Core entity) to RouteDto
            CreateMap<Route, RouteDto>()
                .ForMember(dest => dest.LocationIds, opt => opt.MapFrom(src => src.LocationRoutes.Select(lr => lr.LocationId)));

            // Mapping from CreateRouteDto to Route (EF Core entity)
            CreateMap<CreateRouteDto, Route>()
                .ForMember(dest => dest.RouteId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom((src, dest, destMember, context) => (Guid?)context.Items["UserId"]))
                .ForMember(dest => dest.CreatedByNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.LocationRoutes, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LocationRoutes, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.LocationRoutes = src.LocationIds.Select(id => new LocationRoute
                    {
                        LocationId = id,
                    }).ToList();
                })
                ;

            // Mapping from UpdateRouteDto to Route (EF Core entity)
            CreateMap<UpdateRouteDto, Route>()
                .ForMember(dest => dest.RouteId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.LocationRoutes, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
