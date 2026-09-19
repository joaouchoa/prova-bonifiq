using Microsoft.EntityFrameworkCore;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public class CustomerRepository : EfPagedRepository<Customer>, ICustomerRepository
	{
		public CustomerRepository(TestDbContext ctx) : base(ctx)
		{
		}

		protected override IQueryable<Customer> OrderedQuery => Ctx.Customers.OrderBy(c => c.Id);

		public async Task<Customer?> GetByIdAsync(int id) => await Ctx.Customers.FindAsync(id);
	}
}
