using Microsoft.EntityFrameworkCore;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly TestDbContext _ctx;

		public OrderRepository(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<Order> AddAsync(Order order)
		{
			var entity = (await _ctx.Orders.AddAsync(order)).Entity;
			await _ctx.SaveChangesAsync();
			return entity;
		}

		public async Task<int> CountByCustomerSinceAsync(int customerId, DateTime since) =>
			await _ctx.Orders.CountAsync(o => o.CustomerId == customerId && o.OrderDate >= since);

		public async Task<bool> CustomerHasOrdersAsync(int customerId) =>
			await _ctx.Customers.CountAsync(c => c.Id == customerId && c.Orders.Any()) > 0;
	}
}
