using System;
using System.Collections.Generic;

namespace Academy2.Data;

public partial class Attendance
{
    public int Student { get; set; }

    public long Lesson { get; set; }

    public bool Present { get; set; }

    public virtual Schedule LessonNavigation { get; set; } = null!;

    public virtual Students StudentNavigation { get; set; } = null!;
}
