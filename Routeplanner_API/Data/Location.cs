using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("locations")]
public partial class Location
{
    [Key]
    [Column("location_id")]
    public int LocationId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Location")]
    public virtual LocationDetail? LocationDetail { get; set; }

    [InverseProperty("Location")]
    public virtual ICollection<LocationRoute> LocationRoutes { get; set; } = new List<LocationRoute>();

    [InverseProperty("Location")]
    public virtual ICollection<OpeningTime> OpeningTimes { get; set; } = new List<OpeningTime>();

    [ForeignKey("UserId")]
    [InverseProperty("Locations")]
    public virtual User? User { get; set; }
}
