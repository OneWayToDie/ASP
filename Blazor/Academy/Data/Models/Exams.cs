using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

[PrimaryKey("Student", "Discipline")]
public partial class Exams
{
    [Key]
    [Column("student")]
    public int Student { get; set; }

    [Key]
    [Column("discipline")]
    public short Discipline { get; set; }

    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("grade")]
    public byte? Grade { get; set; }

    [ForeignKey("Discipline")]
    [InverseProperty("Exams")]
    public virtual Disciplines? DisciplineNavigation { get; set; }

    [ForeignKey("Student")]
    [InverseProperty("Exams")]
    public virtual Students? StudentNavigation { get; set; }
}
