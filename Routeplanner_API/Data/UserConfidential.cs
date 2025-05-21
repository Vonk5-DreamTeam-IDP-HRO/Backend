using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class UserConfidential : IdentityUser
{
    public Guid UserId { get; init; }

    public string Email { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
