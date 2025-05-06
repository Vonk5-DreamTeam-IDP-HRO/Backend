using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("location_details")]
[Index("LocationId", Name = "location_details_location_id_key", IsUnique = true)]
public partial class LocationDetail
{
    [Key]
    [Column("location_details_id")]
    public int LocationDetailsId { get; set; }

    [Column("location_id")]
    public int LocationId { get; set; }

    [Column("address")]
    [StringLength(255)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("country")]
    [StringLength(100)]
    public string? Country { get; set; }

    [Column("zip_code")]
    [StringLength(6)]
    public string? ZipCode { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("website")]
    [StringLength(255)]
    public string? Website { get; set; }

    [Column("category")]
    [StringLength(100)]
    public string? Category { get; set; }

    [Column("accessibility")]
    [StringLength(255)]
    public string? Accessibility { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("LocationDetail")]
    public virtual Location Location { get; set; } = null!;
}
