using ProvaPub.Services;
using Xunit;

namespace Test
{
	public class CustomerServiceListTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(999)]
		public void ListCustomers_IgnoresPage_ReturnsAllSeededCustomers(int page)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new CustomerService(ctx);

			var result = sut.ListCustomers(page);

			Assert.Equal(20, result.Customers.Count);
			Assert.Equal(10, result.TotalCount);
			Assert.False(result.HasNext);
		}
	}
}
