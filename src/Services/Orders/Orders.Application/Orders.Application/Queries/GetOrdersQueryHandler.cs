using Common.CQRS;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Abstractions;
using Orders.Application.DTO;
using Orders.Core.Entities;

namespace Orders.Application.Queries;

public class GetOrdersQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await dbContext.Orders.CountAsync(cancellationToken);

        var orders = await dbContext.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var orderSummaries = orders.Select(o => new OrderSummaryDto(
            Id: o.Id.Value,
            OrderName: o.OrderName.Value,
            CustomerId: o.CustomerId.Value,
            Status: o.Status,
            TotalAmount: o.TotalPrice,
            ItemCount: o.OrderItems.Count,
            CreatedAt: o.CreatedAt ?? DateTime.UtcNow
        )).ToList();

        return new GetOrdersResult(
            Orders: orderSummaries,
            TotalCount: totalCount,
            Page: query.Page,
            PageSize: query.PageSize
        );
    }
}

