using Common.CQRS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orders.Infrastructure.Contexts;

namespace Orders.Infrastructure.Decorators;

public class OutboxCommandHandlerDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _innerHandler;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OutboxCommandHandlerDecorator<TCommand, TResponse>> _logger;

    public OutboxCommandHandlerDecorator(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ApplicationDbContext dbContext,
        ILogger<OutboxCommandHandlerDecorator<TCommand, TResponse>> logger)
    {
        _innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OutboxCommandHandlerDecorator invoked for {CommandType}", command.GetType().Name);

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var response = await _innerHandler.Handle(command, cancellationToken);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = command.GetType().Name,
            Payload = JsonConvert.SerializeObject(command),
            OccurredOn = DateTime.UtcNow,
            Processed = false
        };

        _dbContext.OutboxMessages.Add(outboxMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}

