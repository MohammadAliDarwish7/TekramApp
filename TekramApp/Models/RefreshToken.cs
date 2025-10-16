using System;
using System.Collections.Generic;

namespace TekramApp.Models;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
