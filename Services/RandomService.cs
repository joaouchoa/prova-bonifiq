using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class RandomService
	{
		private const int MaxAttempts = 5;

		private readonly TestDbContext _ctx;

		public RandomService(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<int> GetRandom()
		{
			for (var attempt = 0; attempt < MaxAttempts; attempt++)
			{
				var number = await TryInsertUniqueNumberAsync();
				if (number.HasValue)
					return number.Value;
			}

			throw new InvalidOperationException($"Não foi possível gerar um número aleatório único após {MaxAttempts} tentativas.");
		}

		private async Task<int?> TryInsertUniqueNumberAsync()
		{
			var number = Random.Shared.Next();
			var entity = new RandomNumber { Number = number };
			_ctx.Numbers.Add(entity);

			try
			{
				await _ctx.SaveChangesAsync();
				return number;
			}
			catch (DbUpdateException)
			{
				_ctx.Entry(entity).State = EntityState.Detached;
				return null;
			}
		}

	}
}
