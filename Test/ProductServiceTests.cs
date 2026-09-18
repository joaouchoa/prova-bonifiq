using ProvaPub.Services;
using Xunit;

namespace Test
{
	public class ProductServiceTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(999)]
		public void ListProducts_IgnoresPage_ReturnsAllSeededProducts(int page)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new ProductService(ctx);

			var result = sut.ListProducts(page);

			Assert.Equal(20, result.Products.Count);
			Assert.Equal(10, result.TotalCount);
			Assert.False(result.HasNext);
		}
	}
}
