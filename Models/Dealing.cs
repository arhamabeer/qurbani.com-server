using System;
using System.Collections.Generic;

namespace Qurabani.com_Server.Models;

public partial class Dealing
{
    public int DealId { get; set; }

    public int? PersonId { get; set; }

    public int? Adid { get; set; }

    public int? PartId { get; set; }

    public int? QurbaniDay { get; set; }

    public string? Description { get; set; }

    public bool? IsConfirm { get; set; }

    public bool? PickedUp { get; set; }

    public string? Memo { get; set; }

    public virtual AnimalDetail? Ad { get; set; }

    public virtual AnimalPart? Part { get; set; }

    public virtual Person? Person { get; set; }
}
