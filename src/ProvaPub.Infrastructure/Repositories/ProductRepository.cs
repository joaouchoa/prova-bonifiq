using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public class ProductRepository : EfPagedRepository<Product>, IProductRepository
	{
		public ProductRepository(TestDbContext ctx) : base(ctx)
		{
		}

		protected override IQueryable<Product> OrderedQuery => Ctx.Products.OrderBy(p => p.Id);
	}
}
