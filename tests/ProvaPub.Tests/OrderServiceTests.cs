using FluentAssertions;
using Moq;
using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Payments;
using ProvaPub.Application.Services;
using ProvaPub.Application.Validators;
using ProvaPub.Domain;
using Xunit;

namespace ProvaPub.Tests
{
    public class OrderServiceTests
    {
        private static readonly OrderRequestValidator Validator = new();

        private static OrderRequest ValidRequest(string paymentMethod = "pix", decimal paymentValue = 150m, int customerId = 7) =>
            new(paymentMethod, paymentValue, customerId);

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
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            strategy.Setup(s => s.ProcessAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            var result = await sut.PayOrder(ValidRequest());

            // Assert
            result.CustomerId.Should().Be(7);
            result.Value.Should().Be(150m);
        }

        [Fact]
        public async Task PayOrder_PersistsUtcNowButReturnsOrderDateConvertedToBrazilTime()
        {
            // Arrange
            Order? persisted = null;
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Callback<Order>(o => persisted = o)
                .ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);
            var before = DateTime.UtcNow;

            // Act
            var result = await sut.PayOrder(ValidRequest());
            var after = DateTime.UtcNow;

            // Assert
            persisted.Should().NotBeNull();
            persisted!.OrderDate.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
            result.OrderDate.Should().Be(persisted.OrderDate.AddHours(-3));
        }

        [Fact]
        public async Task PayOrder_PersistsTheOrderThroughTheRepository()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            await sut.PayOrder(ValidRequest());

            // Assert
            repository.Verify(r => r.AddAsync(It.Is<Order>(o => o.CustomerId == 7 && o.Value == 150m)), Times.Once);
        }

        [Theory]
        [InlineData("pix")]
        [InlineData("creditcard")]
        [InlineData("paypal")]
        public async Task PayOrder_ResolvesAndProcessesTheStrategyMatchingThePaymentMethod(string paymentMethod)
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customerRepository = MockExistingCustomer(7);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve(paymentMethod)).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            await sut.PayOrder(ValidRequest(paymentMethod: paymentMethod));

            // Assert
            resolver.Verify(r => r.Resolve(paymentMethod), Times.Once);
            strategy.Verify(s => s.ProcessAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task PayOrder_UnsupportedPaymentMethod_PropagatesResolverExceptionWithoutPersisting()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            var customerRepository = MockExistingCustomer(7);
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("bitcoin")).Throws(new ArgumentException("Forma de pagamento 'bitcoin' não é suportada."));
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            var act = () => sut.PayOrder(ValidRequest(paymentMethod: "bitcoin"));

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
            repository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task PayOrder_UnknownCustomer_ThrowsWithoutResolvingStrategyOrPersisting()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            var customerRepository = MockCustomerRepositoryReturning(null);
            var resolver = new Mock<IPaymentStrategyResolver>();
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            var act = () => sut.PayOrder(ValidRequest(customerId: 999));

            // Assert
            var assertion = await act.Should().ThrowAsync<ArgumentException>();
            assertion.Which.ParamName.Should().Be("request");
            assertion.Which.Message.Should().StartWith(string.Format(ValidationMessages.CustomerNotFound, 999));
            resolver.Verify(r => r.Resolve(It.IsAny<string>()), Times.Never);
            repository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task PayOrder_ReturnsOrderWithCustomerAssigned()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
            var customer = new Customer { Id = 7, Name = "Test Customer" };
            var customerRepository = MockCustomerRepositoryReturning(customer);
            var strategy = new Mock<IPaymentStrategy>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            resolver.Setup(r => r.Resolve("pix")).Returns(strategy.Object);
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            var result = await sut.PayOrder(ValidRequest());

            // Assert
            result.Customer.Should().NotBeNull();
            result.Customer!.Id.Should().Be(7);
            result.Customer.Name.Should().Be("Test Customer");
        }

        [Theory]
        [InlineData("pix", 150, 0)]
        [InlineData("pix", 0, 7)]
        [InlineData("pi", 150, 7)]
        public async Task PayOrder_InvalidRequest_ThrowsValidationExceptionWithoutTouchingCustomerRepositoryOrResolver(
            string paymentMethod, decimal paymentValue, int customerId)
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();
            var resolver = new Mock<IPaymentStrategyResolver>();
            var sut = new OrderService(repository.Object, customerRepository.Object, resolver.Object, Validator);

            // Act
            var act = () => sut.PayOrder(new OrderRequest(paymentMethod, paymentValue, customerId));

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            customerRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            resolver.Verify(r => r.Resolve(It.IsAny<string>()), Times.Never);
            repository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }
    }
}
