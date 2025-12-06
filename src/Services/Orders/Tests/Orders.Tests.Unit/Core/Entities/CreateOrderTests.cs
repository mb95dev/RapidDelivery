using NSubstitute.ExceptionExtensions;
using Orders.Core.Entities;
using Orders.Core.Events;
using Orders.Core.Exceptions;
using Orders.Core.ValueObjects;
using Shouldly;

namespace Orders.Tests.Unit.Core.Entities
{
    public class CreateOrderTests
    {
        public Order Act(OrderId id, CustomerId customerId, OrderName orderName, Address shippingAddress, Address billingAddress, Payment payment)
                      => Order.Create(id, customerId, orderName, shippingAddress, billingAddress, payment);

        [Fact]
        public void given_order_id_customer_id_order_name_shipping_address_billing_address_payment()
        {
            var id = OrderId.Of(Guid.NewGuid());
            var customerId = CustomerId.Of(Guid.NewGuid());
            var orderName = OrderName.Of("Szoping");
            var shippingAddress = Address.Of("Mateusz", "Niewiadomwski", "mateusz@wp.pl", "Krakow 1", "Polska", "Malopolska", "37-123");
            var billingAddress = Address.Of("Bartosz", "Nowik", "bartosz@o2.pl", "Rzeszow 2", "Polska", "Podkarpackie", "32-125");
            var payment = Payment.Of("Mastercard", "3213 3521 3214 3233", "23/09", "295", 1);

            var order = Act(id, customerId, orderName, shippingAddress, billingAddress, payment);

            order.ShouldNotBeNull();
            order.Id.ShouldBe(id);
            order.CustomerId.ShouldBe(customerId);
            order.OrderName.ShouldBe(orderName);
            order.ShippingAddress.ShouldBe(shippingAddress);
            order.BillingAddress.ShouldBe(billingAddress);
            order.Payment.ShouldBe(payment);
            order.DomainEvents.Count().ShouldBe(1);


            var @event = order.DomainEvents.Single();
            @event.ShouldBeOfType<OrderCreatedEvent>();
        }

        [Fact]
        public void given_empty_order_name_orders_should_throw_exception()
        {
             var id = OrderId.Of(Guid.NewGuid());
                
        

            //var exception = Record.Exception(() => Act(id, customerId, orderName, shippingAddress, billingAddress, payment));

            //exception.ShouldNotBeNull();
            //exception.ShouldBeOfType<DomainException>();
                

        }
                    

    }
}
