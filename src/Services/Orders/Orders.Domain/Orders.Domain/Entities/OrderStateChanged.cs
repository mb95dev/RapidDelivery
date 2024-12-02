using Orders.Core.Events;

namespace Orders.Core.Entities;

public class OrderStateChanged(Order order) : IDomainEvent
{

}
