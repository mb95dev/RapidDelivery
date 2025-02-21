﻿using Common.Messaging.Events;
using MassTransit;
using MediatR;
using Orders.Application.Commands;
using Orders.Application.DTO;
using Orders.Core.Entities;
using Orders.Core.Events;

namespace Orders.Application.Events.Integration;

public class BasketCheckoutEvenHandler(ISender sender) : IConsumer<BasketCheckoutEvent>, IEvent
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {

        var command = MapToCreateOrderCommand(context.Message);

        await sender.Send(command);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
    {
        var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine, message.Country, message.State, message.ZipCode);
        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.CVV, message.PaymentMethod);
        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: message.CustomerId,
            OrderName: message.UserName,
            ShippingAddress: addressDto,
            BillingAddress: addressDto,
            Payment: paymentDto,
            Status: OrderStatus.New,
            OrderItems: new List<OrderItemDto>
            {

                new OrderItemDto(orderId, new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"), 2, 500),
                new OrderItemDto(orderId, new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), 1, 400)
                
            });
        

        return new CreateOrderCommand(orderDto);
    }
}

