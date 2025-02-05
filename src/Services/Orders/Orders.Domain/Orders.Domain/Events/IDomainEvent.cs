using MediatR;

namespace Orders.Core.Events;

public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}

public interface IEvent : IRequest<Unit> { }
public interface IEvent<TResponse> : IRequest<TResponse> { }