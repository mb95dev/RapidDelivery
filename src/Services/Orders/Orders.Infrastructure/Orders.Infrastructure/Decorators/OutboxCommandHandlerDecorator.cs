using MediatR;
using Newtonsoft.Json;
using Orders.Infrastructure.Contexts;

public class OutboxDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _innerHandler;
    private readonly OrdersDbContext _dbContext;

    public OutboxDecorator(IRequestHandler<TRequest, TResponse> innerHandler, OrdersDbContext dbContext)
    {
        _innerHandler = innerHandler;
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        // Start a transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Execute the original handler logic
        var response = await _innerHandler.Handle(request, cancellationToken);

        // Save the outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = request.GetType().Name, // Use the request type as the event type
            Payload = JsonConvert.SerializeObject(request), // Serialize the request data
            OccurredOn = DateTime.UtcNow,
            Processed = false
        };
        _dbContext.OutboxMessages.Add(outboxMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Commit the transaction
        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}