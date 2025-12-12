using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.DTO;
using Orders.Core.Entities;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Orders.Tests.Integration;

// Integration tests for Orders API endpoints
// Uses Testcontainers for database and message broker isolation
public class OrdersApiTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;

    public OrdersApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        _client?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateOrder_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateOrderRequest(
            new OrderDto(
                Id: Guid.Empty,
                CustomerId: Guid.NewGuid(),
                OrderName: "Integration Test Order",
                ShippingAddress: new AddressDto(
                    "Jane",
                    "Smith",
                    "jane.smith@example.com",
                    "456 Oak Ave",
                    "USA",
                    "NY",
                    "67890"
                ),
                BillingAddress: new AddressDto(
                    "Jane",
                    "Smith",
                    "jane.smith@example.com",
                    "456 Oak Ave",
                    "USA",
                    "NY",
                    "67890"
                ),
                Payment: new PaymentDto(
                    "Jane Smith",
                    "9876543210987654",
                    "06/26",
                    "456",
                    1
                ),
                Status: OrderStatus.New,
                OrderItems: new List<OrderItemDto>
                {
                    new OrderItemDto(
                        Guid.Empty,
                        Guid.NewGuid(),
                        3,
                        15.99m
                    )
                }
            )
        );

        // Act
        var response = await _client.PostAsJsonAsync("/orders", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/orders?page=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<IReadOnlyList<OrderSummaryDto>>();
        orders.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetOrdersSummary_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/orders/summary");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<OrdersSummaryResult>();
        summary.ShouldNotBeNull();
        summary.TotalOrders.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetOrderById_NonExistentOrder_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}

// Helper records matching API responses
public record CreateOrderRequest(OrderDto Order);
public record CreateOrderResponse(Guid Id);
public record OrderSummaryDto(
    Guid Id,
    string OrderName,
    Guid CustomerId,
    OrderStatus Status,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt);
public record OrdersSummaryResult(
    int TotalOrders,
    decimal TotalRevenue,
    Dictionary<int, int> OrdersByStatus);

