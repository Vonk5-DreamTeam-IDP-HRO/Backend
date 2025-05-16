using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class Location
{
    public Guid LocationId { get; init; }

    public Guid? UserId { get; init; }

    public string Name { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; set; }

    public virtual LocationDetail? LocationDetail { get; set; }

    public virtual ICollection<LocationRoute> LocationRoutes { get; set; } = new List<LocationRoute>();

    public virtual ICollection<OpeningTime> OpeningTimes { get; set; } = new List<OpeningTime>();

    public virtual User? User { get; set; }
}
