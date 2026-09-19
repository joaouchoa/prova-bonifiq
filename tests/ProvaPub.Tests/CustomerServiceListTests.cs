using FluentAssertions;
using FluentValidation;
using Moq;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Services;
using ProvaPub.Domain;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceListTests
    {
        [Fact]
        public async Task ListCustomers_RequestsRepositoryWithGivenPageAndDefaultPageSize()
        {
            // Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<Customer>(), 0));
            var sut = new CustomerService(customerRepository.Object, Mock.Of<IOrderRepository>(), Mock.Of<IClock>(), Mock.Of<IValidator<CanPurchaseRequest>>());

            // Act
            await sut.ListCustomers(2);

            // Assert
            customerRepository.Verify(r => r.GetPagedAsync(2, 10), Times.Once);
        }

        [Fact]
        public async Task ListCustomers_MapsRepositoryResultIntoPagedResult()
        {
            // Arrange
            var items = new List<Customer> { new() { Id = 11, Name = "A" }, new() { Id = 12, Name = "B" } };
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetPagedAsync(2, 10)).ReturnsAsync((items, 25));
            var sut = new CustomerService(customerRepository.Object, Mock.Of<IOrderRepository>(), Mock.Of<IClock>(), Mock.Of<IValidator<CanPurchaseRequest>>());

            // Act
            var result = await sut.ListCustomers(2);

            // Assert
            result.Items.Should().BeSameAs(items);
            result.Page.Should().Be(2);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(25);
        }

        [Theory]
        [InlineData(2, 25, true)]
        [InlineData(3, 25, false)]
        [InlineData(1, 10, false)]
        public async Task ListCustomers_ComputesHasNextFromPageTimesPageSizeVersusTotalCount(int page, int totalCount, bool expectedHasNext)
        {
            // Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetPagedAsync(page, 10)).ReturnsAsync((new List<Customer>(), totalCount));
            var sut = new CustomerService(customerRepository.Object, Mock.Of<IOrderRepository>(), Mock.Of<IClock>(), Mock.Of<IValidator<CanPurchaseRequest>>());

            // Act
            var result = await sut.ListCustomers(page);

            // Assert
            result.HasNext.Should().Be(expectedHasNext);
        }
    }
}
