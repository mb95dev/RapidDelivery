using MediatR;
using Newtonsoft.Json;
using Orders.Application.Events;
using Orders.Core.Events;
using Orders.Infrastructure.Contexts;

public class OutboxEventHandlerDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IEvent<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _innerHandler;
    private readonly ApplicationDbContext _dbContext;

    public OutboxEventHandlerDecorator(IRequestHandler<TRequest, TResponse> innerHandler, ApplicationDbContext dbContext)
    {
        _innerHandler = innerHandler;
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var response = await _innerHandler.Handle(request, cancellationToken);


        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = request.GetType().Name,
            Payload = JsonConvert.SerializeObject(request),
            OccurredOn = DateTime.UtcNow,
            Processed = false
        };
        _dbContext.OutboxMessages.Add(outboxMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}