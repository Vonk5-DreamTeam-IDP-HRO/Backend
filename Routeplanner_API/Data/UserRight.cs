using Microsoft.AspNetCore.Identity;

namespace Routeplanner_API.Models
{
    /// <summary>
    /// Represents a user permission or role in the system.
    /// Inherits from IdentityRole with Guid as the key type.
    /// </summary>
    public partial class UserPermission : IdentityRole<Guid>
    {
        // The primary key 'Id' (Guid) is inherited from IdentityRole<Guid>.
        // The role name property 'Name' (string?) is also inherited from IdentityRole<Guid>.
        // If you had a database column "user_right_name", ensure the inherited 'Name' property is mapped to it in DbContext.
        // For example: entity.Property(e => e.Name).HasColumnName("user_right_name");


        // Use the inherited 'Name' property for the role name.

        /// <summary>
        /// Gets or sets the collection of users assigned to this permission/role.
        /// </summary>
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}