
//using Common.CQRS;
//using MediatR;


//namespace Orders.Infrastructure.Decorators
//{
//    internal sealed class OutboxCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
//        where TCommand : class, ICommand, ICommand<Unit>
//    {
//        private readonly ICommandHandler<TCommand> _handler;
//        private readonly IMessageOutbox _outbox;
//        private readonly string _messageId;
//        private readonly bool _enabled;

//        public OutboxCommandHandlerDecorator(ICommandHandler<TCommand> handler, IMessageOutbox outbox,
//            OutboxOptions outboxOptions, IMessagePropertiesAccessor messagePropertiesAccessor)
//        {
//            _handler = handler;
//            _outbox = outbox;
//            _enabled = outboxOptions.Enabled;

//            var messageProperties = messagePropertiesAccessor.MessageProperties;
//            _messageId = string.IsNullOrWhiteSpace(messageProperties?.MessageId)
//                ? Guid.NewGuid().ToString("N")
//                : messageProperties.MessageId;
//        }

//        public Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
//            => _enabled
//                ? _outbox.HandleAsync(_messageId, () => _handler.Handle(command))
//                : _handler.Handle(command);

        
//    }
//}
