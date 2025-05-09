using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("user_confidential", Schema = "auth")]
[Index("Email", Name = "user_confidential_email_key", IsUnique = true)]
public partial class UserConfidential
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserConfidential")]
    public virtual User User { get; set; } = null!;
}
