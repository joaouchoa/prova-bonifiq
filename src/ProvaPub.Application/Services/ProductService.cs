using ProvaPub.Application.DTO.Response;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;

namespace ProvaPub.Application.Services
{
	public class ProductService : PagedListService<Product>
	{
		public ProductService(IProductRepository productRepository) : base(productRepository)
		{
		}

		public Task<PagedResult<Product>> ListProducts(int page) => GetPageAsync(page);
	}
}
