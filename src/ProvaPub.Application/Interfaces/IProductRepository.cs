using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces
{
	public interface IProductRepository
	{
		Task<List<Product>> GetAllAsync();
	}
}
