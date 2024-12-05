namespace Orders.Infrastructure.Contexts;

public class OutboxMessage
{
    public Guid Id { get; set; } 
    public string EventType { get; set; }
    public string Payload { get; set; } 
    public DateTime OccurredOn { get; set; } 
    public bool Processed { get; set; }

    public OutboxMessage(Guid id, string eventType, string payload, DateTime occurredOn, bool processed)
    {
        Id = id;
        EventType = eventType;
        Payload = payload;
        OccurredOn = occurredOn;
        Processed = processed;

    }

    public  OutboxMessage()
    {
        
    }
}