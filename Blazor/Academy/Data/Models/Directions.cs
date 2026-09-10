using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Academy.Data.Models;

public partial class Directions
{
    [Key]
    [Column("direction_id")]
    public byte DirectionId { get; set; }

    [Column("direction_name")]
    [StringLength(50)]
    public string? DirectionName { get; set; }

    [InverseProperty("DirectionNavigation")]
    public virtual ICollection<Groups> Groups { get; set; } = new List<Groups>();
}
