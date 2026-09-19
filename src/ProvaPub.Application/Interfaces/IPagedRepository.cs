namespace ProvaPub.Application.Interfaces
{
	public interface IPagedRepository<T>
	{
		Task<(List<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
	}
}
