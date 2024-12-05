using Orders.Core.Exceptions;

namespace Orders.Core.ValueObjects;

public record OrderId(Guid Value)
{
    public static OrderId Create() => new(Guid.NewGuid());
    public static implicit operator Guid(OrderId date)
        => date.Value;
    
    public static implicit operator OrderId(Guid value)
        => new(value);

    public static OrderId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("OrderId cannot be empty.");
        }

        return new OrderId(value);
    }
}
