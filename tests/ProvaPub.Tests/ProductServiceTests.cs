using FluentAssertions;
using Moq;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Services;
using ProvaPub.Domain;
using Xunit;

namespace ProvaPub.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task ListProducts_RequestsRepositoryWithGivenPageAndDefaultPageSize()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((new List<Product>(), 0));
            var sut = new ProductService(repository.Object);

            // Act
            await sut.ListProducts(2);

            // Assert
            repository.Verify(r => r.GetPagedAsync(2, 10), Times.Once);
        }

        [Fact]
        public async Task ListProducts_MapsRepositoryResultIntoPagedResult()
        {
            // Arrange
            var items = new List<Product> { new() { Id = 11, Name = "A" }, new() { Id = 12, Name = "B" } };
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetPagedAsync(2, 10)).ReturnsAsync((items, 25));
            var sut = new ProductService(repository.Object);

            // Act
            var result = await sut.ListProducts(2);

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
        public async Task ListProducts_ComputesHasNextFromPageTimesPageSizeVersusTotalCount(int page, int totalCount, bool expectedHasNext)
        {
            // Arrange
            var repository = new Mock<IProductRepository>();
            repository.Setup(r => r.GetPagedAsync(page, 10)).ReturnsAsync((new List<Product>(), totalCount));
            var sut = new ProductService(repository.Object);

            // Act
            var result = await sut.ListProducts(page);

            // Assert
            result.HasNext.Should().Be(expectedHasNext);
        }
    }
}
