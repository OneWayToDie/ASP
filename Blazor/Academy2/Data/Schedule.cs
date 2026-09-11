using System;
using System.Collections.Generic;

namespace Academy2.Data;

public partial class Schedule
{
    public long LessonId { get; set; }

    public int Group { get; set; }

    public short Discipline { get; set; }

    public short Teacher { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? Time { get; set; }

    public bool Spent { get; set; }

    public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();

    public virtual Disciplines DisciplineNavigation { get; set; } = null!;

    public virtual ICollection<Grades> Grades { get; set; } = new List<Grades>();

    public virtual Groups GroupNavigation { get; set; } = null!;

    public virtual Teachers TeacherNavigation { get; set; } = null!;
}
