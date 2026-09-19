using ProvaPub.Application.DTO.Response;
using ProvaPub.Application.Interfaces;

namespace ProvaPub.Application.Services
{
	public abstract class PagedListService<TEntity>
	{
		protected const int DefaultPageSize = 10;

		private readonly IPagedRepository<TEntity> _repository;

		protected PagedListService(IPagedRepository<TEntity> repository)
		{
			_repository = repository;
		}

		protected async Task<PagedResult<TEntity>> GetPageAsync(int page)
		{
			var (items, totalCount) = await _repository.GetPagedAsync(page, DefaultPageSize);
			return new PagedResult<TEntity>
			{
				Items = items,
				Page = page,
				PageSize = DefaultPageSize,
				TotalCount = totalCount
			};
		}
	}
}
