using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Academy2.Data;

public partial class Holidays
{
    [Key]
    public byte HolidayId { get; set; }

    public string HolidayName { get; set; } = null!;

    public byte Duration { get; set; }

    public byte? Month { get; set; }

    public byte? Day { get; set; }

    public virtual ICollection<DaysOff> DaysOff { get; set; } = new List<DaysOff>();
}
