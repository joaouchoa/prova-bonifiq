using FluentValidation;
using Moq;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Services;
using ProvaPub.Application.Validators;
using ProvaPub.Domain;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private static readonly IValidator<CanPurchaseRequest> Validator = new CanPurchaseRequestValidator();

        private static readonly DateTime Wednesday1200Utc = new(2026, 9, 16, 12, 0, 0, DateTimeKind.Utc);

        private static DateTime UtcTimeForHour(int hour) =>
            new DateTime(2026, 9, 16, hour, 0, 0, DateTimeKind.Utc).AddHours(3);

        private static CanPurchaseRequest Request(int customerId, decimal purchaseValue) =>
            new(customerId, purchaseValue);

        private static Mock<IClock> MockClock(DateTime utcNow)
        {
            var clock = new Mock<IClock>();
            clock.Setup(c => c.UtcNow).Returns(utcNow);
            return clock;
        }

        private static Mock<ICustomerRepository> MockExistingCustomer(int customerId) =>
            MockCustomerRepositoryReturning(new Customer { Id = customerId, Name = "Test Customer" });

        private static Mock<ICustomerRepository> MockCustomerRepositoryReturning(Customer? customer)
        {
            var repository = new Mock<ICustomerRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(customer);
            return repository;
        }

        private static Mock<IOrderRepository> MockOrderRepository(int ordersThisMonth = 0, bool hasBoughtBefore = true)
        {
            var repository = new Mock<IOrderRepository>();
            repository.Setup(r => r.CountByCustomerSinceAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(ordersThisMonth);
            repository.Setup(r => r.CustomerHasOrdersAsync(It.IsAny<int>())).ReturnsAsync(hasBoughtBefore);
            return repository;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CanPurchase_InvalidCustomerId_ThrowsWithoutTouchingRepositories(int customerId)
        {
            var customerRepository = new Mock<ICustomerRepository>();
            var orderRepository = new Mock<IOrderRepository>();
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            await Assert.ThrowsAsync<ValidationException>(() => sut.CanPurchase(Request(customerId, 50)));

            customerRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            orderRepository.Verify(r => r.CountByCustomerSinceAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task CanPurchase_InvalidPurchaseValue_ThrowsWithoutTouchingRepositories(decimal purchaseValue)
        {
            var customerRepository = new Mock<ICustomerRepository>();
            var orderRepository = new Mock<IOrderRepository>();
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            await Assert.ThrowsAsync<ValidationException>(() => sut.CanPurchase(Request(1, purchaseValue)));

            customerRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            orderRepository.Verify(r => r.CountByCustomerSinceAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task CanPurchase_CustomerDoesNotExist_ThrowsWithoutTouchingOrderRepository()
        {
            var customerRepository = MockCustomerRepositoryReturning(null);
            var orderRepository = new Mock<IOrderRepository>();
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CanPurchase(Request(9999, 50)));

            orderRepository.Verify(r => r.CountByCustomerSinceAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task CanPurchase_CustomerAlreadyPurchasedThisMonth_ReturnsFalse()
        {
            var customerRepository = MockExistingCustomer(1);
            var orderRepository = MockOrderRepository(ordersThisMonth: 1);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            var result = await sut.CanPurchase(Request(1, 50));

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_FirstPurchaseAboveLimit_ReturnsFalse()
        {
            var customerRepository = MockExistingCustomer(2);
            var orderRepository = MockOrderRepository(ordersThisMonth: 0, hasBoughtBefore: false);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            var result = await sut.CanPurchase(Request(2, 100.01m));

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_FirstPurchaseAtOrBelowLimitDuringBusinessHours_ReturnsTrue()
        {
            var customerRepository = MockExistingCustomer(2);
            var orderRepository = MockOrderRepository(ordersThisMonth: 0, hasBoughtBefore: false);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            var result = await sut.CanPurchase(Request(2, 100m));

            Assert.True(result);
        }

        [Fact]
        public async Task CanPurchase_ReturningCustomerAboveOldLimitDuringBusinessHours_ReturnsTrue()
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository(ordersThisMonth: 0, hasBoughtBefore: true);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 500m));

            Assert.True(result);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(19)]
        public async Task CanPurchase_OutsideBusinessHours_ReturnsFalse(int hour)
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository();
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(UtcTimeForHour(hour)).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 50));

            Assert.False(result);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(18)]
        public async Task CanPurchase_AtBusinessHoursBoundaries_ReturnsTrue(int hour)
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository();
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(UtcTimeForHour(hour)).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 50));

            Assert.True(result);
        }

        [Fact]
        public async Task CanPurchase_UtcEveningThatIsWithinBusinessHours_ReturnsTrue()
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository();
            var utc8Pm = new DateTime(2026, 9, 16, 20, 0, 0, DateTimeKind.Utc);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(utc8Pm).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 50));

            Assert.True(result);
        }

        [Fact]
        public async Task CanPurchase_UtcMorningThatIsBeforeBusinessHours_ReturnsFalse()
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository();
            var utc10Am = new DateTime(2026, 9, 16, 10, 0, 0, DateTimeKind.Utc);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(utc10Am).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 50));

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_Saturday_ReturnsFalse()
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository();
            var saturdayNoon = Wednesday1200Utc.AddDays(3);
            Assert.Equal(DayOfWeek.Saturday, saturdayNoon.DayOfWeek);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(saturdayNoon).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 50));

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_Sunday_ReturnsFalse()
        {
            var customerRepository = MockExistingCustomer(3);
            var orderRepository = MockOrderRepository();
            var sundayNoon = Wednesday1200Utc.AddDays(4);
            Assert.Equal(DayOfWeek.Sunday, sundayNoon.DayOfWeek);
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(sundayNoon).Object, Validator);

            var result = await sut.CanPurchase(Request(3, 50));

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_UsesOneMonthWindowEndingAtClockUtcNow()
        {
            var customerRepository = MockExistingCustomer(1);
            var orderRepository = MockOrderRepository();
            var sut = new CustomerService(customerRepository.Object, orderRepository.Object, MockClock(Wednesday1200Utc).Object, Validator);

            await sut.CanPurchase(Request(1, 50));

            orderRepository.Verify(r => r.CountByCustomerSinceAsync(1, Wednesday1200Utc.AddMonths(-1)), Times.Once);
        }
    }
}
