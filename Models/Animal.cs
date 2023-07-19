using System;
using System.Collections.Generic;

namespace Qurabani.com_Server.Models;

public partial class Animal
{
    public int AnimalId { get; set; }

    public string? Type { get; set; }

    public string? Memo { get; set; }

    public int? Parts { get; set; }

    public virtual ICollection<AnimalDetail> AnimalDetails { get; set; } = new List<AnimalDetail>();
}
