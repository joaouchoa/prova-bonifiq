using ProvaPub.Models;
using ProvaPub.Services;
using Xunit;

namespace Test
{
	public class CustomerServiceCanPurchaseTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public async Task CanPurchase_InvalidCustomerId_Throws(int customerId)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new CustomerService(ctx);

			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.CanPurchase(customerId, 50));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-10)]
		public async Task CanPurchase_InvalidPurchaseValue_Throws(decimal purchaseValue)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new CustomerService(ctx);

			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.CanPurchase(1, purchaseValue));
		}

		[Fact]
		public async Task CanPurchase_CustomerDoesNotExist_Throws()
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new CustomerService(ctx);

			await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CanPurchase(9999, 50));
		}

		[Fact]
		public async Task CanPurchase_CustomerAlreadyPurchasedThisMonth_ReturnsFalse()
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			ctx.Orders.Add(new Order { CustomerId = 1, Value = 20, OrderDate = DateTime.UtcNow.AddDays(-5) });
			ctx.SaveChanges();
			var sut = new CustomerService(ctx);

			var result = await sut.CanPurchase(1, 50);

			Assert.False(result);
		}

		[Fact]
		public async Task CanPurchase_FirstPurchaseAboveLimit_ReturnsFalse()
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new CustomerService(ctx);

			var result = await sut.CanPurchase(2, 100.01m);

			Assert.False(result);
		}
	}
}
