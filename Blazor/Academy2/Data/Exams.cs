using System;
using System.Collections.Generic;

namespace Academy2.Data;

public partial class Exams
{
    public int Student { get; set; }

    public short Discipline { get; set; }

    public DateOnly? Date { get; set; }

    public byte? Grade { get; set; }

    public virtual Disciplines DisciplineNavigation { get; set; } = null!;

    public virtual Students StudentNavigation { get; set; } = null!;
}
