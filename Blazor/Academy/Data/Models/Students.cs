using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Students
{
    [Key]
    [Column("stud_id")]
    public int StudId { get; set; }

    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("middle_name")]
    [StringLength(50)]
    public string? MiddleName { get; set; }

    [Column("birth_date")]
    public DateOnly BirthDate { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(16)]
    public string? Phone { get; set; }

    [Column("photo", TypeName = "image")]
    public byte[]? Photo { get; set; }

    [Column("group")]
    public int? Group { get; set; }

    [InverseProperty("StudentNavigation")]
    public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();

    [InverseProperty("StudentNavigation")]
    public virtual ICollection<Exams> Exams { get; set; } = new List<Exams>();

    [InverseProperty("StudentNavigation")]
    public virtual ICollection<Grades> Grades { get; set; } = new List<Grades>();

    [ForeignKey("Group")]
    [InverseProperty("Students")]
    public virtual Groups? GroupNavigation { get; set; }
}
