using ProvaPub.Application.Services;
using ProvaPub.Infrastructure.Repositories;
using Xunit;

namespace ProvaPub.RegressionTests
{
	public class ProductServiceTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(999)]
		public async Task ListProducts_IgnoresPage_ReturnsAllSeededProducts(int page)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new ProductService(new ProductRepository(ctx));

			var result = await sut.ListProducts(page);

			Assert.Equal(20, result.Products.Count);
			Assert.Equal(10, result.TotalCount);
			Assert.False(result.HasNext);
		}
	}
}
