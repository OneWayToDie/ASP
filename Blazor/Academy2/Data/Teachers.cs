using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Academy2.Data;

public partial class Teachers
{
	[Key]
	public short TeacherId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public byte[]? Photo { get; set; }

    public DateOnly? WorkSince { get; set; }

    public decimal? Rate { get; set; }

    public virtual ICollection<Salary> Salary { get; set; } = new List<Salary>();

    public virtual ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();
}
