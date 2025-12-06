using Common.CQRS;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orders.Infrastructure.Contexts;

public class OutboxEventHandlerDecorator<TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{
    private readonly INotificationHandler<TEvent> _innerHandler;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OutboxEventHandlerDecorator<TEvent>> _logger;

    public OutboxEventHandlerDecorator(
        INotificationHandler<TEvent> innerHandler,
        ApplicationDbContext dbContext,
        ILogger<OutboxEventHandlerDecorator<TEvent>> logger)
    {
        _innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OutboxEventHandlerDecorator invoked for {EventType}", notification.GetType().Name);

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        await _innerHandler.Handle(notification, cancellationToken);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = notification.GetType().Name,
            Payload = JsonConvert.SerializeObject(notification),
            OccurredOn = DateTime.UtcNow,
            Processed = false
        };

        _dbContext.OutboxMessages.Add(outboxMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
//public class OutboxEventHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
//    where TQuery : IQuery<TResponse>
//    where TResponse : notnull
//{
//    private readonly IQueryHandler<TQuery, TResponse> _innerHandler;
//    private readonly ApplicationDbContext _dbContext;

//    public OutboxEventHandlerDecorator(IQueryHandler<TQuery, TResponse> innerHandler, ApplicationDbContext dbContext)
//    {
//        _innerHandler = innerHandler;
//        _dbContext = dbContext;
//    }

//    public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken)
//    {
//        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

//        var response = await _innerHandler.Handle(request, cancellationToken);


//        var outboxMessage = new OutboxMessage
//        {
//            Id = Guid.NewGuid(),
//            EventType = request.GetType().Name,
//            Payload = JsonConvert.SerializeObject(request),
//            OccurredOn = DateTime.UtcNow,
//            Processed = false
//        };
//        _dbContext.OutboxMessages.Add(outboxMessage);

//        await _dbContext.SaveChangesAsync(cancellationToken);

//        await transaction.CommitAsync(cancellationToken);

//        return response;
//    }
//}