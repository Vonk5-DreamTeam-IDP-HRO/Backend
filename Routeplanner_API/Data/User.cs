using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class User
{
    public Guid UserId { get; init; }

    public string Username { get; set; } = null!;

    public int UserRightId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; init; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual UserPermission Right { get; set; } = null!;

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();

    public virtual UserConfidential? UserConfidential { get; set; }
}
