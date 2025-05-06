using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("location_route")]
[Index("RouteId", "LocationId", Name = "location_route_route_id_location_id_key", IsUnique = true)]
public partial class LocationRoute
{
    [Key]
    [Column("location_route_id")]
    public int LocationRouteId { get; set; }

    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("location_id")]
    public int LocationId { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("LocationRoutes")]
    public virtual Location Location { get; set; } = null!;

    [ForeignKey("RouteId")]
    [InverseProperty("LocationRoutes")]
    public virtual Route Route { get; set; } = null!;
}
