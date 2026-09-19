using Moq;
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
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<Customer>(), 0));
            var sut = new CustomerService(customerRepository.Object, Mock.Of<IOrderRepository>());

            await sut.ListCustomers(2);

            customerRepository.Verify(r => r.GetPagedAsync(2, 10), Times.Once);
        }

        [Fact]
        public async Task ListCustomers_MapsRepositoryResultIntoPagedResult()
        {
            var items = new List<Customer> { new() { Id = 11, Name = "A" }, new() { Id = 12, Name = "B" } };
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetPagedAsync(2, 10)).ReturnsAsync((items, 25));
            var sut = new CustomerService(customerRepository.Object, Mock.Of<IOrderRepository>());

            var result = await sut.ListCustomers(2);

            Assert.Same(items, result.Items);
            Assert.Equal(2, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(25, result.TotalCount);
        }

        [Theory]
        [InlineData(2, 25, true)]
        [InlineData(3, 25, false)]
        [InlineData(1, 10, false)]
        public async Task ListCustomers_ComputesHasNextFromPageTimesPageSizeVersusTotalCount(int page, int totalCount, bool expectedHasNext)
        {
            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetPagedAsync(page, 10)).ReturnsAsync((new List<Customer>(), totalCount));
            var sut = new CustomerService(customerRepository.Object, Mock.Of<IOrderRepository>());

            var result = await sut.ListCustomers(page);

            Assert.Equal(expectedHasNext, result.HasNext);
        }
    }
}
