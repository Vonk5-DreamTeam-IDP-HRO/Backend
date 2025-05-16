using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class UserPermission
{
    public int UserRightId { get; init; }

    public string UserRightName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
