using System;
using System.Collections.Generic;

namespace Academy2.Data;

public partial class Salary
{
    public long PaymentId { get; set; }

    public short Teacher { get; set; }

    public decimal Accrued { get; set; }

    public bool Received { get; set; }

    public virtual Teachers TeacherNavigation { get; set; } = null!;
}
