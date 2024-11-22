namespace Orders.Core.Entities;

public class AggregateId : IEquatable<AggregateId>
{
    public Guid Value { get; }

    public AggregateId()
    {

    }

    public AggregateId(Guid value)
    {
        Value = value;
    }

    public bool Equals(AggregateId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) || Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((AggregateId)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

