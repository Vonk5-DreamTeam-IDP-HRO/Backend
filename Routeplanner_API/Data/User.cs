using Microsoft.AspNetCore.Identity;

namespace Routeplanner_API.Models
{
    /// <summary>
    /// Represents an application user with extended properties and navigation relationships.
    /// Inherits from IdentityUser with a Guid as the primary key.
    /// </summary>
    public partial class User : IdentityUser<Guid>
    {
        // UserId is inherited as 'Id' from IdentityUser<Guid>s

        /// <summary>
        /// Gets or sets the username for the user. Overrides the base class property.
        /// </summary>
        public override string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the foreign key linking the user to a specific user permission.
        /// </summary>
        public Guid UserRightId { get; set; }

        /// <summary>
        /// Gets or sets the hashed password for the user. Overrides the base class property.
        /// </summary>
        public override string? PasswordHash { get; set; }

        /// <summary>
        /// Gets the timestamp indicating when the user was created.
        /// </summary>
        public DateTime? CreatedAt { get; init; }

        /// <summary>
        /// Gets or sets the collection of locations associated with this user.
        /// </summary>
        public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

        /// <summary>
        /// Gets or sets the user’s permission level.
        /// </summary>
        public virtual UserPermission Right { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of routes created by this user.
        /// </summary>
        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
    }
}
