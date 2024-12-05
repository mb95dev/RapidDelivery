using Orders.Core.Exceptions;

namespace Orders.Core.ValueObjects;

public record ProductId
{
    public Guid Value { get; }
 //   private ProductId(Guid value) => Value = value;
 public static ProductId Create() => new(Guid.NewGuid());
 public static implicit operator Guid(ProductId date)
     => date.Value;
    
 public static implicit operator ProductId(Guid value)
     => new(value);

    public static ProductId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("ProductId cannot be empty.");
        }

        return new ProductId(value);
    }
}