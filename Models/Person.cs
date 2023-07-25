using System;
using System.Collections.Generic;

namespace Qurabani.com_Server.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string? Name { get; set; }

    public string? Contact { get; set; }

    public string? EmergencyContact { get; set; }

    public string? Address { get; set; }

    public string? Nic { get; set; }

    public string? Memo { get; set; }

    public virtual ICollection<Dealing> Dealings { get; set; } = new List<Dealing>();
}
