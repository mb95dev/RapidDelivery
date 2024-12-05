using Orders.Core.Entities;


namespace Orders.Core.Events;
public record OrderUpdatedEvent(Order order) : IDomainEvent;