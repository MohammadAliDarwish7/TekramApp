using System;
using System.Collections.Generic;

namespace TekramApp.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public Guid ShopId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public bool Availability { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Shop Shop { get; set; } = null!;
}
