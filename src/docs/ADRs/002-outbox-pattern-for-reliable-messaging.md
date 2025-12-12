# ADR-002: Outbox Pattern for Reliable Messaging

## Status
Accepted

## Context
In a microservices architecture, we need to ensure reliable message delivery when publishing domain events. The challenge is maintaining transactional consistency between:
1. Saving domain changes to the database
2. Publishing events to the message broker

If we publish directly and the database transaction fails, we have inconsistent state. If we save first and then publish, a crash between save and publish loses the event.

## Decision
We will implement the **Outbox Pattern** to ensure reliable message delivery.

## How It Works

1. **Domain Event Raised**: When an aggregate changes state, it raises a domain event
2. **Outbox Message Created**: A decorator intercepts the command handler and creates an OutboxMessage record
3. **Same Transaction**: Both the domain change and OutboxMessage are saved in the same database transaction
4. **Background Processor**: A background service (OutboxProcessor) periodically:
   - Reads unprocessed OutboxMessages
   - Publishes them to RabbitMQ
   - Marks them as processed

## Rationale

### Advantages
- **Transactional Guarantee**: Domain changes and event recording are atomic
- **At-Least-Once Delivery**: Events are persisted, so they can be retried
- **Idempotency**: Consumers can handle duplicate events
- **No Lost Events**: Even if the service crashes, events are preserved

### Alternatives Considered

**Direct Publishing**
- ❌ No transactional guarantee
- ❌ Events lost if transaction fails
- ✅ Simpler implementation

**Two-Phase Commit (2PC)**
- ❌ Complex to implement
- ❌ Performance overhead
- ❌ Not supported by all message brokers

**Transaction Log Tailing**
- ✅ Decouples publishing from business logic
- ❌ Requires database-specific CDC tools
- ❌ More infrastructure complexity

## Consequences

### Positive
- Reliable event delivery
- No lost events
- Simple to understand and maintain

### Negative
- Slight delay in event delivery (background processing)
- Additional database table (OutboxMessages)
- Need to handle duplicate events in consumers

## Implementation Details

```csharp
// Decorator pattern intercepts command handlers
public class OutboxCommandHandlerDecorator<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
{
    // Saves OutboxMessage in same transaction as domain changes
}
```

## References
- [Outbox Pattern - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications#the-outbox-pattern)
- [Reliable Messaging Patterns](https://microservices.io/patterns/data/transactional-outbox.html)

