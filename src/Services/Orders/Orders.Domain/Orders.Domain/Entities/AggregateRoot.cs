using Orders.Core.Events;

namespace Orders.Core.Entities;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _events = new();
    public IEnumerable<IDomainEvent> Events => _events;

    public Guid Id
    {
        get;
        protected set;
    }
    public int Version { get; protected set; }

    protected void AddEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void ClearEvents() => _events.Clear();

}

