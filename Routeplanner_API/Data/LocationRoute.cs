using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class LocationRoute
{
    public Guid LocationRouteId { get; init; }

    public Guid RouteId { get; set; }

    public Guid LocationId { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Route Route { get; set; } = null!;
}
