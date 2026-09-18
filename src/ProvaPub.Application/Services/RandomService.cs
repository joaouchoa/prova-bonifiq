using ProvaPub.Application.Interfaces;

namespace ProvaPub.Application.Services
{
	public class RandomService
	{
		private const int MaxAttempts = 5;

		private readonly IRandomNumberRepository _repository;

		public RandomService(IRandomNumberRepository repository)
		{
			_repository = repository;
		}

		public async Task<int> GetRandom()
		{
			for (var attempt = 0; attempt < MaxAttempts; attempt++)
			{
				var number = Random.Shared.Next();
				if (await _repository.TryAddAsync(number))
					return number;
			}

			throw new InvalidOperationException($"Não foi possível gerar um número aleatório único após {MaxAttempts} tentativas.");
		}
	}
}
