namespace Orders.Application.DTO;

public record OrderItemDto(Guid OrderId, Guid ProductId, int Quantity, decimal Price);

