using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Routeplanner_API.Models;

public partial class UserPermission : IdentityRole<Guid>
{
    // The primary key 'Id' (Guid) is inherited from IdentityRole<Guid>.
    // The role name property 'Name' (string?) is also inherited from IdentityRole<Guid>.
    // If you had a database column "user_right_name", ensure the inherited 'Name' property is mapped to it in DbContext.
    // For example: entity.Property(e => e.Name).HasColumnName("user_right_name");


    // Use the inherited 'Name' property for the role name.

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
