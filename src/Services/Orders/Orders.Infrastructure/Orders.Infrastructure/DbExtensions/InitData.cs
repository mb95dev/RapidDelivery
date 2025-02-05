using Orders.Core.Entities;
using Orders.Core.ValueObjects;

namespace Orders.Infrastructure.DbExtensions
{
    internal class InitData
    {

        public static IEnumerable<Customer> Customers =>
        new List<Customer>
        {
        Customer.Create(CustomerId.Of(new Guid("58c49479-ec65-4de2-86e7-033c546291a5")), "mateusz", "mateusz@gmail.com"),
        Customer.Create(CustomerId.Of(new Guid("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d")), "karol", "karol@gmail.com")
        };

        public static IEnumerable<Product> Products =>
            new List<Product>
            {
            Product.Create(ProductId.Of(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61")), "IPhone X", 500),
            Product.Create(ProductId.Of(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914")), "Samsung 10", 400),
            Product.Create(ProductId.Of(new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8")), "Huawei Plus", 650),
            Product.Create(ProductId.Of(new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27")), "Xiaomi Mi", 450)
            };

        public static IEnumerable<Order> OrdersWithItems
        {
            get
            {
                var firstAddress = Address.Of("mateusz", "dorek", "mateusz@gmail.com", "Maja 3/1", "Poland", "Rzeszow", "38050");
                var secondAddress = Address.Of("karol", "korek", "karol@gmail.com", "Grunwaldzka 5/8", "Poland", "Krakow", "08050");

                var payment1 = Payment.Of("mateusz", "5555555555554444", "12/28", "355", 1);
                var payment2 = Payment.Of("karol", "8885555555554444", "06/30", "222", 2);

                var order1 = Order.Create(
                                OrderId.Of(Guid.NewGuid()),
                                CustomerId.Of(new Guid("58c49479-ec65-4de2-86e7-033c546291a5")),
                                OrderName.Of("ORD_1"),
                                shippingAddress: firstAddress,
                                billingAddress: firstAddress,
                                payment1);
                order1.Add(ProductId.Of(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61")), 2, 500);
                order1.Add(ProductId.Of(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914")), 1, 400);

                var order2 = Order.Create(
                                OrderId.Of(Guid.NewGuid()),
                                CustomerId.Of(new Guid("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d")),
                                OrderName.Of("ORD_2"),
                                shippingAddress: secondAddress,
                                billingAddress: secondAddress,
                                payment2);
                order2.Add(ProductId.Of(new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8")), 1, 650);
                order2.Add(ProductId.Of(new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27")), 2, 450);

                return new List<Order> { order1, order2 };
            }
        }
    }
}
