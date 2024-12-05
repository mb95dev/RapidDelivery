using Orders.Core.Exceptions;

namespace Orders.Core.ValueObjects
{
    public record CustomerId
    {
        public Guid Value { get; }

      // private CustomerId(Guid value) => Value = value;

        
        public static CustomerId Create() => new(Guid.NewGuid());
        public static implicit operator Guid(CustomerId date)
            => date.Value;
    
        public static implicit operator CustomerId(Guid value)
            => new(value);

        public static CustomerId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("CustomerId cannot be empty.");
            }

            return new CustomerId(value);
        }
    }
}
