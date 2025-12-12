using Carter;
using MediatR;
using Orders.Application.Queries;
using Orders.Application.DTO;

namespace Orders.API.Endpoints.GetOrders;

public class GetOrders : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async (int? page, int? pageSize, ISender sender) =>
        {
            var query = new GetOrdersQuery(
                Page: page ?? 1,
                PageSize: pageSize ?? 10
            );

            var result = await sender.Send(query);

            return Results.Ok(result.Orders);
        })
        .WithName("GetOrders")
        .Produces<IReadOnlyList<OrderSummaryDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Orders")
        .WithDescription("Get paginated list of orders");
    }
}

