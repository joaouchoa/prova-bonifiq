namespace ProvaPub.Application.Interfaces
{
	public interface IRandomNumberRepository
	{
		Task<bool> TryAddAsync(int number);
	}
}
