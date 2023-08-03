using System;
using System.Collections.Generic;

namespace Qurabani.com_Server.Models;

public partial class AuthAdmin
{
    public int? PersonId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Salt { get; set; }
}
