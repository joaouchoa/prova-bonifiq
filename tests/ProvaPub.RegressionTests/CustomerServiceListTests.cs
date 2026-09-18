using ProvaPub.Application.Services;
using ProvaPub.Infrastructure.Repositories;
using Xunit;

namespace ProvaPub.RegressionTests
{
	public class CustomerServiceListTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(999)]
		public async Task ListCustomers_IgnoresPage_ReturnsAllSeededCustomers(int page)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new CustomerService(new CustomerRepository(ctx), new OrderRepository(ctx));

			var result = await sut.ListCustomers(page);

			Assert.Equal(20, result.Customers.Count);
			Assert.Equal(10, result.TotalCount);
			Assert.False(result.HasNext);
		}
	}
}
