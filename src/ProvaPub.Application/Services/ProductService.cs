using ProvaPub.Application.Common;
using ProvaPub.Application.Interfaces;

namespace ProvaPub.Application.Services
{
	public class ProductService
	{
		private readonly IProductRepository _productRepository;

		public ProductService(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<ProductList> ListProducts(int page)
		{
			var products = await _productRepository.GetAllAsync();
			return new ProductList() { HasNext = false, TotalCount = 10, Products = products };
		}

	}
}
