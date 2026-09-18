using Microsoft.EntityFrameworkCore;
using ProvaPub.Services;
using Xunit;

namespace Test
{
	public class OrderServiceTests
	{
		[Theory]
		[InlineData("pix")]
		[InlineData("creditcard")]
		[InlineData("paypal")]
		[InlineData("bitcoin")]
		public async Task PayOrder_NeverPersistsOrder_RegardlessOfPaymentMethod(string paymentMethod)
		{
			using var ctx = TestDbContextFactory.CreateInMemory();
			var sut = new OrderService(ctx);

			var order = await sut.PayOrder(paymentMethod, 150m, customerId: 3);

			Assert.Equal(150m, order.Value);
			Assert.Equal(0, order.CustomerId);
			Assert.Equal(0, await ctx.Orders.CountAsync());
		}
	}
}
