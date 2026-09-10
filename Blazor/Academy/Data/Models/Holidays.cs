using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Holidays
{
    [Key]
    [Column("holiday_id")]
    public byte HolidayId { get; set; }

    [Column("holiday_name")]
    [StringLength(150)]
    public string HolidayName { get; set; } = null!;

    [Column("duration")]
    public byte Duration { get; set; }

    [Column("month")]
    public byte? Month { get; set; }

    [Column("day")]
    public byte? Day { get; set; }

    [InverseProperty("HolidayNavigation")]
    public virtual ICollection<DaysOff> DaysOff { get; set; } = new List<DaysOff>();
}
