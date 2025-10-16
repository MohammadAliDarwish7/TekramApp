using System;
using System.Collections.Generic;

namespace TekramApp.Models;

public partial class City
{
    public Guid Id { get; set; }

    public Guid CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Country Country { get; set; } = null!;
}
