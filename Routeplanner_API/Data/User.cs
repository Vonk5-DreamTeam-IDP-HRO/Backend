using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("users", Schema = "auth")]
[Index("Username", Name = "users_username_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("right_id")]
    public int RightId { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    [ForeignKey("RightId")]
    [InverseProperty("Users")]
    public virtual UserRight Right { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();

    [InverseProperty("User")]
    public virtual UserConfidential? UserConfidential { get; set; }
}
