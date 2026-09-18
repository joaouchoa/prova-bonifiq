using Microsoft.EntityFrameworkCore;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly TestDbContext _ctx;

		public ProductRepository(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<List<Product>> GetAllAsync() => await _ctx.Products.ToListAsync();
	}
}
