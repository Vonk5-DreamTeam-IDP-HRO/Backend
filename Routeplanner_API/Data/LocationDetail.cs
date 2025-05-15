using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class LocationDetail
{
    public Guid LocationDetailsId { get; init; }

    public Guid LocationId { get; init; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? ZipCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Website { get; set; }

    public string? Category { get; set; }

    public string? Accessibility { get; set; }

    public virtual Location Location { get; set; } = null!;
}
