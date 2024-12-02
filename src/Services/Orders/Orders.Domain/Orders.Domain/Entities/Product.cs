﻿using Orders.Core.ValueObjects;

namespace Orders.Core.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; } = default!;

    public static Product Create(ProductId id, string name, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        var product = new Product
        {
            Id = id.Value,
            Name = name,
            Price = price
        };

        return product;
    }
}