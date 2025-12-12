using Common.CQRS;
using Orders.Core.Entities;

namespace Orders.Application.Queries;

public record GetOrdersSummaryQuery() : IQuery<OrdersSummaryResult>;

public record OrdersSummaryResult(
    int TotalOrders,
    decimal TotalRevenue,
    Dictionary<int, int> OrdersByStatus);

