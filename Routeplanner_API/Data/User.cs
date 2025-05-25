using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class User : IdentityUser<Guid>
{
    // UserId is inherited as 'Id' from IdentityUser<Guid>

    public override string? UserName { get; set; }

    public Guid UserRightId { get; set; }

    public override string? PasswordHash { get; set; }

    public DateTime? CreatedAt { get; init; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual UserPermission Right { get; set; } = null!;

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
}
