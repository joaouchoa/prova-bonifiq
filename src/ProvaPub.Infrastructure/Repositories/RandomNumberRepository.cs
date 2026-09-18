using Microsoft.EntityFrameworkCore;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.Infrastructure.Repositories
{
	public class RandomNumberRepository : IRandomNumberRepository
	{
		private readonly TestDbContext _ctx;

		public RandomNumberRepository(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<bool> TryAddAsync(int number)
		{
			var entity = new RandomNumber { Number = number };
			_ctx.Numbers.Add(entity);

			try
			{
				await _ctx.SaveChangesAsync();
				return true;
			}
			catch (DbUpdateException)
			{
				_ctx.Entry(entity).State = EntityState.Detached;
				return false;
			}
		}
	}
}
