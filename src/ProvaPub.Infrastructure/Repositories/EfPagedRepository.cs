using Microsoft.EntityFrameworkCore;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public abstract class EfPagedRepository<T> where T : class
	{
		protected readonly TestDbContext Ctx;

		protected EfPagedRepository(TestDbContext ctx)
		{
			Ctx = ctx;
		}

		protected abstract IQueryable<T> OrderedQuery { get; }

		public async Task<(List<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
		{
			var totalCount = await OrderedQuery.CountAsync();
			var skip = Math.Max(0, (page - 1) * pageSize);
			var items = await OrderedQuery.Skip(skip).Take(pageSize).ToListAsync();
			return (items, totalCount);
		}
	}
}
