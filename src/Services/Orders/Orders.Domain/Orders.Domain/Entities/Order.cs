using Orders.Core.Events;

namespace Orders.Core.Entities;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string OrderName { get; private set; }
    public string CancellationReason { get; private set; }

    public Order(AggregateId id, Guid customerId, OrderStatus status, DateTime createdAt, string orderName, string cancellationReason = "", DateTime? deliveryDate = null)
    {
        Id = id.Value;
        CustomerId = customerId;
        Status = status;
        DeliveryDate = deliveryDate;
        CreatedAt = createdAt;
        OrderName = orderName;
        CancellationReason = cancellationReason;
    }

    public static Order Create(AggregateId id, Guid customerId, OrderStatus status, string orderName, DateTime createdAt)
    {
        var order = new Order(id, customerId, status, createdAt, orderName);
        order.AddEvent(new OrderCreatedEvent(order));

        return order;
    }

}

