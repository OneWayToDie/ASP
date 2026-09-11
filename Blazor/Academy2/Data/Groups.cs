using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Academy2.Data;

public partial class Groups
{
    [Key]
    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public byte? Direction { get; set; }

    public byte? Weekdays { get; set; }

    public TimeOnly? StartTime { get; set; }

    public DateOnly? StartDate { get; set; }

    public virtual Directions? DirectionNavigation { get; set; }

    public virtual ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();

    public virtual ICollection<Students> Students { get; set; } = new List<Students>();
}
