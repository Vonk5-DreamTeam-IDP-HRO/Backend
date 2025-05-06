using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

[Table("opening_times")]
public partial class OpeningTime
{
    [Key]
    [Column("opening_id")]
    public int OpeningId { get; set; }

    [Column("location_id")]
    public int LocationId { get; set; }

    [Column("day_of_week")]
    [StringLength(10)]
    public string DayOfWeek { get; set; } = null!;

    [Column("open_time")]
    public TimeOnly? OpenTime { get; set; }

    [Column("close_time")]
    public TimeOnly? CloseTime { get; set; }

    [Column("is_24_hours")]
    public bool? Is24Hours { get; set; }

    [Column("timezone")]
    [StringLength(10)]
    public string? Timezone { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("OpeningTimes")]
    public virtual Location Location { get; set; } = null!;
}
