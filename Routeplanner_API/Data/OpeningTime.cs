using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class OpeningTime
{
    public Guid OpeningId { get; init; }

    public Guid LocationId { get; init; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public bool? Is24Hours { get; set; }

    public string? Timezone { get; set; }

    public virtual Location Location { get; set; } = null!;
}
