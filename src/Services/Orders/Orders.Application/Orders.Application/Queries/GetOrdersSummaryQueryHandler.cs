using Common.CQRS;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Abstractions;
using Orders.Core.Entities;

namespace Orders.Application.Queries;

public class GetOrdersSummaryQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetOrdersSummaryQuery, OrdersSummaryResult>
{
    public async Task<OrdersSummaryResult> Handle(GetOrdersSummaryQuery query, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .Include(o => o.OrderItems)
            .ToListAsync(cancellationToken);

        var totalOrders = orders.Count;
        var totalRevenue = orders.Sum(o => o.TotalPrice);

        var ordersByStatus = orders
            .GroupBy(o => (int)o.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all statuses are represented
        foreach (OrderStatus status in Enum.GetValues<OrderStatus>())
        {
            if (!ordersByStatus.ContainsKey((int)status))
            {
                ordersByStatus[(int)status] = 0;
            }
        }

        return new OrdersSummaryResult(
            TotalOrders: totalOrders,
            TotalRevenue: totalRevenue,
            OrdersByStatus: ordersByStatus
        );
    }
}

