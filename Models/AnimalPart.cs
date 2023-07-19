using System;
using System.Collections.Generic;

namespace Qurabani.com_Server.Models;

public partial class AnimalPart
{
    public int PartId { get; set; }

    public int? Part { get; set; }

    public string? Memo { get; set; }

    public virtual ICollection<Dealing> Dealings { get; set; } = new List<Dealing>();
}
