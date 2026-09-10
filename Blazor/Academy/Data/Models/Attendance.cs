using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

[PrimaryKey("Student", "Lesson")]
public partial class Attendance
{
    [Key]
    [Column("student")]
    public int Student { get; set; }

    [Key]
    [Column("lesson")]
    public long Lesson { get; set; }

    [Column("present")]
    public bool Present { get; set; }

    [ForeignKey("Lesson")]
    [InverseProperty("Attendance")]
    public virtual Schedule? LessonNavigation { get; set; }

    [ForeignKey("Student")]
    [InverseProperty("Attendance")]
    public virtual Students? StudentNavigation { get; set; }
}
