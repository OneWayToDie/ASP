using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

[Table("DaysOFF")]
public partial class DaysOff
{
    [Key]
    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("holiday")]
    public byte Holiday { get; set; }

    [ForeignKey("Holiday")]
    [InverseProperty("DaysOff")]
    public virtual Holidays? HolidayNavigation { get; set; }
}
