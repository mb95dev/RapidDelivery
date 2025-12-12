using MassTransit;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Orders.Application.Abstractions;
using Orders.Application.Commands;
using Orders.Application.DTO;
using Orders.Core.Entities;
using Orders.Core.ValueObjects;
using Shouldly;
using Xunit;

namespace Orders.Tests.Unit.Application.Handlers;

// Unit tests for CreateOrderHandler
// Demonstrates AAA pattern (Arrange, Act, Assert) and mocking strategies
public class CreateOrderHandlerTests
{
    private readonly CreateOrderHandler _handler;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderHandlerTests()
    {
        _dbContext = Substitute.For<IApplicationDbContext>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _handler = new CreateOrderHandler(_dbContext, _publishEndpoint);
    }

    [Fact]
    public async Task Handle_ValidOrder_ShouldCreateOrderAndReturnId()
    {
        // Arrange
        var orderDto = CreateValidOrderDto();
        var command = new CreateOrderCommand(orderDto);
        var ordersSet = Substitute.For<DbSet<Order>>();
        
        _dbContext.Orders.Returns(ordersSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        
        // Verify order was added to context
        await ordersSet.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidOrder_ShouldCreateOrderWithCorrectProperties()
    {
        // Arrange
        var orderDto = CreateValidOrderDto();
        var command = new CreateOrderCommand(orderDto);
        var ordersSet = Substitute.For<DbSet<Order>>();
        Order? capturedOrder = null;

        _dbContext.Orders.Returns(ordersSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        
        ordersSet.AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                capturedOrder = callInfo.Arg<Order>();
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedOrder.ShouldNotBeNull();
        capturedOrder.OrderName.Value.ShouldBe(orderDto.OrderName);
        capturedOrder.CustomerId.Value.ShouldBe(orderDto.CustomerId);
        capturedOrder.Status.ShouldBe(OrderStatus.New);
        capturedOrder.OrderItems.Count.ShouldBe(orderDto.OrderItems.Count);
    }

    [Fact]
    public async Task Handle_OrderWithMultipleItems_ShouldCreateAllItems()
    {
        // Arrange
        var orderDto = CreateValidOrderDto();
        orderDto.OrderItems.Add(new OrderItemDto(
            Guid.Empty,
            Guid.NewGuid(),
            5,
            25.50m
        ));
        
        var command = new CreateOrderCommand(orderDto);
        var ordersSet = Substitute.For<DbSet<Order>>();
        Order? capturedOrder = null;

        _dbContext.Orders.Returns(ordersSet);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        
        ordersSet.AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                capturedOrder = callInfo.Arg<Order>();
            });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedOrder.ShouldNotBeNull();
        capturedOrder.OrderItems.Count.ShouldBe(2);
    }

    private static OrderDto CreateValidOrderDto()
    {
        return new OrderDto(
            Id: Guid.Empty,
            CustomerId: Guid.NewGuid(),
            OrderName: "Test Order",
            ShippingAddress: new AddressDto(
                "John",
                "Doe",
                "john.doe@example.com",
                "123 Main St",
                "USA",
                "CA",
                "12345"
            ),
            BillingAddress: new AddressDto(
                "John",
                "Doe",
                "john.doe@example.com",
                "123 Main St",
                "USA",
                "CA",
                "12345"
            ),
            Payment: new PaymentDto(
                "John Doe",
                "1234567890123456",
                "12/25",
                "123",
                1
            ),
            Status: OrderStatus.New,
            OrderItems: new List<OrderItemDto>
            {
                new OrderItemDto(
                    Guid.Empty,
                    Guid.NewGuid(),
                    2,
                    10.99m
                )
            }
        );
    }
}
