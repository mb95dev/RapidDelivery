using Orders.Core.ValueObjects;

namespace Orders.Core.Entities
{
    public class OrderItem : AggregateRoot
    {
        internal OrderItem(OrderId orderId, ProductId productId, int quantity, decimal price)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public OrderId OrderId { get; private set; } = default!;
        public ProductId ProductId { get; private set; } = default!;
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
    }
}
