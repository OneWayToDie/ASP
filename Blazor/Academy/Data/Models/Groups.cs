using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Groups
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Column("group_name")]
    [StringLength(10)]
    public string? GroupName { get; set; }

    [Column("direction")]
    public byte? Direction { get; set; }

    [Column("weekdays")]
    public byte? Weekdays { get; set; }

    [Column("start_time")]
    [Precision(0)]
    public TimeOnly? StartTime { get; set; }

    [Column("start_date")]
    public DateOnly? StartDate { get; set; }

    [ForeignKey("Direction")]
    [InverseProperty("Groups")]
    public virtual Directions? DirectionNavigation { get; set; }

    [InverseProperty("GroupNavigation")]
    public virtual ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();

    [InverseProperty("GroupNavigation")]
    public virtual ICollection<Students> Students { get; set; } = new List<Students>();
}
