using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Academy2.Data;

public partial class Disciplines
{
    public short DisciplineId { get; set; }

    public string DisciplineName { get; set; } = null!;

    public byte NumberOfLessons { get; set; }

    public virtual ICollection<Exams> Exams { get; set; } = new List<Exams>();

    public virtual ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();
}
