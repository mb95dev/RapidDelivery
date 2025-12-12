using Common.CQRS;
using Orders.Application.DTO;

namespace Orders.Application.Queries;

public record GetOrdersQuery(int Page = 1, int PageSize = 10) : IQuery<GetOrdersResult>;

public record GetOrdersResult(IReadOnlyList<OrderSummaryDto> Orders, int TotalCount, int Page, int PageSize);

public record OrderSummaryDto(
    Guid Id,
    string OrderName,
    Guid CustomerId,
    OrderStatus Status,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt);

