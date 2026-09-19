using Moq;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Payments;
using ProvaPub.Application.Services;
using ProvaPub.Domain;
using Xunit;

namespace ProvaPub.Tests
{
    public class OrderServiceTests
    {
        private static Mock<ICustomerRepository> MockExistingCustomer(int customerId) =>
            MockCustomerRepositoryReturning(new Customer { Id = customerId, Name = "Test Customer" });

        private static Mock<ICustomerRepository> MockCustomerRepositoryReturning(Customer? customer)
        {
            var repository = new Mock<ICustomerRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(customer);
            return repository;
        }

        [Fact]
        public async Task PayOrder_AssignsCustomerIdAndValueToTheOrder()
        {
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            strategy.Setup(s => s.ProcessAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            var result = await sut.PayOrder("pix", 150m, customerId: 7);

            Assert.Equal(7, result.CustomerId);
            Assert.Equal(150m, result.Value);
        }

        [Fact]
        public async Task PayOrder_PersistsUtcNowButReturnsOrderDateConvertedToBrazilTime()
        {
            Order? persisted = null;
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Callback<Order>(o => persisted = o)
                .ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            var before = DateTime.UtcNow;
            var result = await sut.PayOrder("pix", 150m, customerId: 7);
            var after = DateTime.UtcNow;

            Assert.NotNull(persisted);
            Assert.InRange(persisted!.OrderDate, before, after); // persistido em UTC
            Assert.Equal(persisted.OrderDate.AddHours(-3), result.OrderDate); // retornado em horário do Brasil
        }

        [Fact]
        public async Task PayOrder_PersistsTheOrderThroughTheRepository()
        {
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            await sut.PayOrder("pix", 150m, customerId: 7);

            repository.Verify(r => r.AddAsync(It.Is<Order>(o => o.CustomerId == 7 && o.Value == 150m)), Times.Once);
        }

        [Theory]
        [InlineData("pix")]
        [InlineData("creditcard")]
        [InlineData("paypal")]
        public async Task PayOrder_ResolvesAndProcessesTheStrategyMatchingThePaymentMethod(string paymentMethod)
        {
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve(paymentMethod)).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            await sut.PayOrder(paymentMethod, 150m, customerId: 7);

            resolver.Verify(r => r.Resolve(paymentMethod), Times.Once);
            strategy.Verify(s => s.ProcessAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task PayOrder_UnsupportedPaymentMethod_PropagatesResolverExceptionWithoutPersisting()
        {
            var repository = new Mock<IOrderRepository>();
            var customerRepository = MockExistingCustomer(7);
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("bitcoin")).Throws(new ArgumentException("Forma de pagamento 'bitcoin' não é suportada."));
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => sut.PayOrder("bitcoin", 150m, customerId: 7));
            repository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task PayOrder_UnknownCustomer_ThrowsWithoutResolvingStrategyOrPersisting()
        {
            var repository = new Mock<IOrderRepository>();
            var customerRepository = MockCustomerRepositoryReturning(null);
            var resolver = new Mock<IPaymentStrategyResolver>();
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => sut.PayOrder("pix", 150m, customerId: 999));

            resolver.Verify(r => r.Resolve(It.IsAny<string>()), Times.Never);
            repository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task PayOrder_ReturnsOrderWithCustomerAssigned()
        {
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customer = new Customer { Id = 7, Name = "Test Customer" };
            var customerRepository = MockCustomerRepositoryReturning(customer);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object);

            var result = await sut.PayOrder("pix", 150m, customerId: 7);

            Assert.NotNull(result.Customer);
            Assert.Equal(7, result.Customer!.Id);
            Assert.Equal("Test Customer", result.Customer.Name);
        }
    }
}
