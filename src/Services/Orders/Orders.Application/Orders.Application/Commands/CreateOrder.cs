using Common.CQRS;
using Orders.Application.DTO;

public record CreateOrder(OrderDto Order)
    : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid Id);