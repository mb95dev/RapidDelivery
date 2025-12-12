using Grpc.Core;
using MediatR;
using Orders.API.Protos;
using Orders.Application.Commands;
using Orders.Application.DTO;
using Orders.Application.Queries;
using Orders.Core.Entities;
using Orders.Core.ValueObjects;

namespace Orders.API.Services;

// gRPC service implementation for Order operations
// Demonstrates both unary and server streaming patterns
public class OrderGrpcService : OrderService.OrderServiceBase
{
    private readonly ISender _sender;
    private readonly ILogger<OrderGrpcService> _logger;

    public OrderGrpcService(ISender sender, ILogger<OrderGrpcService> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    // Unary RPC: Get order by ID
    public override async Task<OrderResponse> GetOrder(
        GetOrderRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetOrder called for OrderId: {OrderId}", request.OrderId);

        if (!Guid.TryParse(request.OrderId, out var orderId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Invalid order ID format"));
        }

        var query = new GetOrderByIdQuery(orderId);
        var orderDto = await _sender.Send(query, context.CancellationToken);

        if (orderDto is null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Order with ID {request.OrderId} not found"));
        }

        return MapToOrderResponse(orderDto);
    }

    // Unary RPC: Create order
    public override async Task<CreateOrderResponse> CreateOrder(
        CreateOrderRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC CreateOrder called for OrderName: {OrderName}", 
            request.Order?.OrderName);

        try
        {
            // Convert gRPC message to application command
            var command = MapToCreateOrderCommand(request.Order);
            var result = await _sender.Send(command, context.CancellationToken);

            return new CreateOrderResponse
            {
                Id = result.Id.ToString(),
                Success = true,
                Message = "Order created successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order via gRPC");
            throw new RpcException(
                new Status(StatusCode.Internal, $"Error creating order: {ex.Message}"));
        }
    }

    // Server streaming: Stream orders as they are created/updated
    public override async Task StreamOrders(
        StreamOrdersRequest request,
        IServerStreamWriter<OrderResponse> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC StreamOrders started");

        var query = new GetOrdersQuery(Page: 1, PageSize: request.PageSize);
        var result = await _sender.Send(query, context.CancellationToken);

        foreach (var orderSummary in result.Orders)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }

            // Get full order details
            var orderQuery = new GetOrderByIdQuery(orderSummary.Id);
            var orderDto = await _sender.Send(orderQuery, context.CancellationToken);

            if (orderDto is not null)
            {
                await responseStream.WriteAsync(MapToOrderResponse(orderDto));
            }

            // Simulate streaming delay
            await Task.Delay(500, context.CancellationToken);
        }

        _logger.LogInformation("gRPC StreamOrders completed");
    }

    // Unary RPC: Get orders summary
    public override async Task<OrdersSummaryResponse> GetOrdersSummary(
        GetOrdersSummaryRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetOrdersSummary called");

        var query = new GetOrdersSummaryQuery();
        var result = await _sender.Send(query, context.CancellationToken);

        var response = new OrdersSummaryResponse
        {
            TotalOrders = result.TotalOrders,
            TotalRevenue = (double)result.TotalRevenue
        };

        foreach (var (status, count) in result.OrdersByStatus)
        {
            response.OrdersByStatus[status] = count;
        }

        return response;
    }

    // Helper methods for mapping between gRPC messages and domain/application models
    private static OrderResponse MapToOrderResponse(Orders.Application.DTO.OrderDto orderDto)
    {
        var response = new OrderResponse
        {
            Id = orderDto.Id.ToString(),
            CustomerId = orderDto.CustomerId.ToString(),
            OrderName = orderDto.OrderName,
            Status = (Protos.OrderStatus)orderDto.Status,
            TotalPrice = (double)orderDto.OrderItems.Sum(oi => oi.Price * oi.Quantity),
            CreatedAt = DateTime.UtcNow.ToString("O")
        };

        // Map order items
        foreach (var item in orderDto.OrderItems)
        {
            response.OrderItems.Add(new OrderItemMessage
            {
                ProductId = item.ProductId.ToString(),
                Quantity = item.Quantity,
                Price = (double)item.Price
            });
        }

        // Map addresses
        response.ShippingAddress = new AddressMessage
        {
            FirstName = orderDto.ShippingAddress.FirstName,
            LastName = orderDto.ShippingAddress.LastName,
            EmailAddress = orderDto.ShippingAddress.EmailAddress,
            AddressLine = orderDto.ShippingAddress.AddressLine,
            Country = orderDto.ShippingAddress.Country,
            State = orderDto.ShippingAddress.State,
            ZipCode = orderDto.ShippingAddress.ZipCode
        };

        response.BillingAddress = new AddressMessage
        {
            FirstName = orderDto.BillingAddress.FirstName,
            LastName = orderDto.BillingAddress.LastName,
            EmailAddress = orderDto.BillingAddress.EmailAddress,
            AddressLine = orderDto.BillingAddress.AddressLine,
            Country = orderDto.BillingAddress.Country,
            State = orderDto.BillingAddress.State,
            ZipCode = orderDto.BillingAddress.ZipCode
        };

        // Map payment
        response.Payment = new PaymentMessage
        {
            CardName = orderDto.Payment.CardName,
            CardNumber = orderDto.Payment.CardNumber,
            Expiration = orderDto.Payment.Expiration,
            Cvv = orderDto.Payment.Cvv,
            PaymentMethod = orderDto.Payment.PaymentMethod
        };

        return response;
    }

    private static Orders.Application.Commands.CreateOrderCommand MapToCreateOrderCommand(
        OrderMessage orderMessage)
    {
        var orderDto = new Orders.Application.DTO.OrderDto(
            Id: Guid.Empty, // Will be generated by handler
            CustomerId: Guid.Parse(orderMessage.CustomerId),
            OrderName: orderMessage.OrderName,
            ShippingAddress: new Orders.Application.DTO.AddressDto(
                orderMessage.ShippingAddress.FirstName,
                orderMessage.ShippingAddress.LastName,
                orderMessage.ShippingAddress.EmailAddress,
                orderMessage.ShippingAddress.AddressLine,
                orderMessage.ShippingAddress.Country,
                orderMessage.ShippingAddress.State,
                orderMessage.ShippingAddress.ZipCode
            ),
            BillingAddress: new Orders.Application.DTO.AddressDto(
                orderMessage.BillingAddress.FirstName,
                orderMessage.BillingAddress.LastName,
                orderMessage.BillingAddress.EmailAddress,
                orderMessage.BillingAddress.AddressLine,
                orderMessage.BillingAddress.Country,
                orderMessage.BillingAddress.State,
                orderMessage.BillingAddress.ZipCode
            ),
            Payment: new Orders.Application.DTO.PaymentDto(
                orderMessage.Payment.CardName,
                orderMessage.Payment.CardNumber,
                orderMessage.Payment.Expiration,
                orderMessage.Payment.Cvv,
                orderMessage.Payment.PaymentMethod
            ),
            Status: OrderStatus.New,
            OrderItems: orderMessage.OrderItems.Select(oi => 
                new Orders.Application.DTO.OrderItemDto(
                    Guid.Empty, // Will be set by handler
                    Guid.Parse(oi.ProductId),
                    oi.Quantity,
                    (decimal)oi.Price
                )).ToList()
        );

        return new Orders.Application.Commands.CreateOrderCommand(orderDto);
    }
}

