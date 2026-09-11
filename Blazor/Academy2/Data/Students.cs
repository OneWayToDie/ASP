using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Academy2.Data;

public partial class Students
{
    [Key]
    public int StudId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly BirthDate { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public byte[]? Photo { get; set; }

    public int? Group { get; set; }

    public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();

    public virtual ICollection<Exams> Exams { get; set; } = new List<Exams>();

    public virtual ICollection<Grades> Grades { get; set; } = new List<Grades>();

    public virtual Groups? GroupNavigation { get; set; }
}
