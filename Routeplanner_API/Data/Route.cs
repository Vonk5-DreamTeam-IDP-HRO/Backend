using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class Route
{
    public Guid RouteId { get; init; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? CreatedBy { get; init; }

    public bool? IsPrivate { get; set; }

    public DateTime? CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<LocationRoute> LocationRoutes { get; set; } = new List<LocationRoute>();
}
