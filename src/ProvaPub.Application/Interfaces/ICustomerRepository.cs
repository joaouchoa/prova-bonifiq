using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces
{
	public interface ICustomerRepository : IPagedRepository<Customer>
	{
		Task<Customer?> GetByIdAsync(int id);
	}
}
