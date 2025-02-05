using MediatR;
using Orders.Core.Entities;

namespace Orders.Core.Events;

public record OrderCreatedEvent(Order Order) : IDomainEvent, IEvent<Unit>;