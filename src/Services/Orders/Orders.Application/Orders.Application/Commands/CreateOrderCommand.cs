using Common.CQRS;
using Orders.Application.DTO;

namespace Orders.Application.Commands;

public record CreateOrderCommand(OrderDto Order)
    : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid Id);