using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces
{
	public interface ICustomerRepository
	{
		Task<Customer?> GetByIdAsync(int id);
		Task<List<Customer>> GetAllAsync();
	}
}
