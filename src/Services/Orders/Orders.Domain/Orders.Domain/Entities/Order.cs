namespace Orders.Core.Entities;

public class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CancellationReason { get; private set; }

    public Order(AggregateId id, Guid customerId, OrderStatus status, DateTime createdAt, string cancellationReason = "", DateTime? deliveryDate = null)
    {
        Id = id;
        CustomerId = customerId;
        Status = status;
        DeliveryDate = deliveryDate;
        CreatedAt = createdAt;
        CancellationReason = cancellationReason;
    }

    public static Order Create(AggregateId id, Guid customerId, OrderStatus status, DateTime createdAt)
    {
        var order = new Order(id, customerId, status, createdAt);
        order.AddEvent(new OrderStateChanged(order));

        return order;
    }

    public void SetDeliveryDate(DateTime deliveryDate)
    {
        DeliveryDate = deliveryDate.Date;
    }
}

