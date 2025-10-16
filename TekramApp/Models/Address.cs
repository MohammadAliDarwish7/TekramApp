using System;
using System.Collections.Generic;

namespace TekramApp.Models;

public partial class Address
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string AddressLine { get; set; } = null!;

    public bool IsDefault { get; set; }

    public Guid CityId { get; set; }

    public Guid CountryId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
