using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.CQRS;
using Orders.Application.DTO;
using Orders.Core.Entities;
using Orders.Core.ValueObjects;

namespace Orders.Application.Commands
{
    public class CreateOrderHandler
        : ICommandHandler<CreateOrder, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrder command, CancellationToken cancellationToken)
        {
            //create Order entity from command object
            //save to database
            //return result 

            var order = CreateNewOrder(command.Order);

            //dbContext.Orders.Add(order);
            //await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateOrderResult(order.Id);
        }

        private Order CreateNewOrder(OrderDto orderDto)
        {
            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(), 2, 49.99m),
                new OrderItemDto(Guid.NewGuid(), Guid.NewGuid(), 1, 299.99m)
            };

            // Create shipping and billing addresses
            var shippingAddress = new AddressDto(
                "John",
                "Doe",
                "john.doe@example.com",
                "123 Main Street",
                "USA",
                "California",
                "90001"
            );

            var billingAddress = new AddressDto(
                "John",
                "Doe",
                "john.doe@example.com",
                "123 Main Street",
                "USA",
                "California",
                "90001"
            );

            // Create payment details
            var payment = new PaymentDto(
                "John Doe",
                "4111111111111111",
                "12/25",
                "123",
                1 // 1 for Credit Card
            );

            // Create the order DTO
            var order = new OrderDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Sample Order",
                shippingAddress,
                billingAddress,
                payment,
                OrderStatus.New,
                orderItems
            );

            var newOrder = Order.Create(
                id: new AggregateId(Guid.NewGuid()),
                customerId: orderDto.CustomerId,
                status: order.Status,
                orderName: order.OrderName,
                createdAt: DateTime.UtcNow
            );

            return newOrder;
        }
    }
}
