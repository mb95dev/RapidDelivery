using MassTransit;
using NSubstitute;
using Orders.Application.Abstractions;
using Orders.Application.Commands;

namespace Orders.Tests.Unit.Application.Handlers
{
    public  class CreateOrderHandlerTests
    {

        private Task Act(CreateOrderCommand command) => _handler.Handle(command, CancellationToken.None);


        #region Arrange

        private readonly CreateOrderHandler _handler;
        private readonly IApplicationDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateOrderHandlerTests()
        {
            _dbContext = Substitute.For<IApplicationDbContext>();
            _publishEndpoint = Substitute.For<IPublishEndpoint>();
            _handler = new CreateOrderHandler(_dbContext, _publishEndpoint);
        }
        #endregion

    }
}
