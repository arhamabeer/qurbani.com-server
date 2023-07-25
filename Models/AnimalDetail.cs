using System;
using System.Collections.Generic;

namespace Qurabani.com_Server.Models;

public partial class AnimalDetail
{
    public int Adid { get; set; }

    public int? AnimalId { get; set; }

    public int? Number { get; set; }

    public decimal? PartSellPrice { get; set; }

    public decimal? PartFinalPrice { get; set; }

    public string? Description { get; set; }

    public string? Memo { get; set; }

    public virtual Animal? Animal { get; set; }

    public virtual ICollection<Dealing> Dealings { get; set; } = new List<Dealing>();
}
