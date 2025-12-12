using Carter;
using MediatR;
using Orders.Application.Queries;
using Orders.Application.DTO;

namespace Orders.API.Endpoints.GetOrderById;

public class GetOrderById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetOrderByIdQuery(id);
            var result = await sender.Send(query);

            if (result is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(result);
        })
        .WithName("GetOrderById")
        .Produces<OrderDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get Order by ID")
        .WithDescription("Get a single order by its ID");
    }
}

