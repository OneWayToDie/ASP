using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Academy2.Data;

public partial class Directions
{
    [Key]
    public byte DirectionId { get; set; }

    public string? DirectionName { get; set; }

    public virtual ICollection<Groups> Groups { get; set; } = new List<Groups>();
}
