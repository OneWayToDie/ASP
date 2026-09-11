using System;
using System.Collections.Generic;

namespace Academy2.Data;

public partial class DaysOff
{
    public DateOnly Date { get; set; }

    public byte Holiday { get; set; }

    public virtual Holidays HolidayNavigation { get; set; } = null!;
}
