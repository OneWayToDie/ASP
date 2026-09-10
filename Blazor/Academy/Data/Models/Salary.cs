using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Salary
{
    [Key]
    [Column("payment_id")]
    public long PaymentId { get; set; }

    [Column("teacher")]
    public short Teacher { get; set; }

    [Column("accrued", TypeName = "smallmoney")]
    public decimal Accrued { get; set; }

    [Column("received")]
    public bool Received { get; set; }

    [ForeignKey("Teacher")]
    [InverseProperty("Salary")]
    public virtual Teachers? TeacherNavigation { get; set; }
}
