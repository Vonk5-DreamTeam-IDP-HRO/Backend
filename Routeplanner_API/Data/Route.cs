using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("routes")]
[Index("IsPrivate", Name = "idx_routes_is_private")]
public partial class Route
{
    [Key]
    [Column("route_id")]
    public int RouteId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("is_private")]
    public bool? IsPrivate { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Routes")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Route")]
    public virtual ICollection<LocationRoute> LocationRoutes { get; set; } = new List<LocationRoute>();
}
