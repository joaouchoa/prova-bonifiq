using Microsoft.EntityFrameworkCore;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly TestDbContext _ctx;

		public CustomerRepository(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<Customer?> GetByIdAsync(int id) => await _ctx.Customers.FindAsync(id);

		public async Task<List<Customer>> GetAllAsync() => await _ctx.Customers.ToListAsync();
	}
}
