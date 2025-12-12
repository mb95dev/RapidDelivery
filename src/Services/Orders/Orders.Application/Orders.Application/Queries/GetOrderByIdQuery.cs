using Common.CQRS;
using Orders.Application.DTO;

namespace Orders.Application.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto?>;

