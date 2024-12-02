using Orders.Application.DTO;
using Orders.Core.Entities;

namespace Orders.Application.Mappers
{
    public static class OrderMapper
    {

        public static OrderDto ToOrderDto(this Order order)
        {
            return DtoFromOrder(order);
        }

        private static OrderDto DtoFromOrder(Order order)
        {
            return new OrderDto(
                        Id: order.Id,
                        CustomerId: order.CustomerId,
                        OrderName: order.OrderName,
                        ShippingAddress: null,
                        BillingAddress: null,
                        Payment: null,
                        Status: order.Status,
                        OrderItems: order.OrderItems.Select(oi => new OrderItemDto(oi.OrderId.Value, oi.ProductId.Value, oi.Quantity, oi.Price)).ToList()
                    );
        }
    }
}
