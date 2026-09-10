using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Schedule
{
    [Key]
    [Column("lesson_id")]
    public long LessonId { get; set; }

    [Column("group")]
    public int Group { get; set; }

    [Column("discipline")]
    public short Discipline { get; set; }

    [Column("teacher")]
    public short Teacher { get; set; }

    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("time")]
    [Precision(0)]
    public TimeOnly? Time { get; set; }

    [Column("spent")]
    public bool Spent { get; set; }

    [InverseProperty("LessonNavigation")]
    public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();

    [ForeignKey("Discipline")]
    [InverseProperty("Schedule")]
    public virtual Disciplines? DisciplineNavigation { get; set; }

    [InverseProperty("LessonNavigation")]
    public virtual ICollection<Grades> Grades { get; set; } = new List<Grades>();

    [ForeignKey("Group")]
    [InverseProperty("Schedule")]
    public virtual Groups? GroupNavigation { get; set; }

    [ForeignKey("Teacher")]
    [InverseProperty("Schedule")]
    public virtual Teachers? TeacherNavigation { get; set; }
}
