using Common.CQRS;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Abstractions;
using Orders.Application.DTO;
using Orders.Application.Mappers;
using Orders.Core.ValueObjects;

namespace Orders.Application.Queries;

public class GetOrderByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == OrderId.Of(query.OrderId), cancellationToken);

        return order?.ToOrderDto();
    }
}

