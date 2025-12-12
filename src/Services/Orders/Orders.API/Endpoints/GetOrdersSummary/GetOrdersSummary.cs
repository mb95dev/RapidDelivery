using Carter;
using MediatR;
using Orders.Application.Queries;
using Orders.Application.DTO;

namespace Orders.API.Endpoints.GetOrdersSummary;

public class GetOrdersSummary : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/summary", async (ISender sender) =>
        {
            var query = new GetOrdersSummaryQuery();
            var result = await sender.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetOrdersSummary")
        .Produces<OrdersSummaryResult>(StatusCodes.Status200OK)
        .WithSummary("Get Orders Summary")
        .WithDescription("Get aggregated statistics about orders for reporting dashboard");
    }
}

