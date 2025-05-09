using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("user_rights", Schema = "auth")]
[Index("RightName", Name = "user_rights_right_name_key", IsUnique = true)]
public partial class UserRight
{
    [Key]
    [Column("right_id")]
    public int RightId { get; set; }

    [Column("right_name")]
    [StringLength(50)]
    public string RightName { get; set; } = null!;

    [InverseProperty("Right")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
