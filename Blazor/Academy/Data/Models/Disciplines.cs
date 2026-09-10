using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Disciplines
{
    [Key]
    [Column("discipline_id")]
    public short DisciplineId { get; set; }

    [Column("discipline_name")]
    [StringLength(150)]
    public string DisciplineName { get; set; } = null!;

    [Column("number_of_lessons")]
    public byte NumberOfLessons { get; set; }

    [InverseProperty("DisciplineNavigation")]
    public virtual ICollection<Exams> Exams { get; set; } = new List<Exams>();

    [InverseProperty("DisciplineNavigation")]
    public virtual ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();
}
