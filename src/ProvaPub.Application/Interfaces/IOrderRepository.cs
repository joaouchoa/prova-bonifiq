using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces
{
	public interface IOrderRepository
	{
		Task<Order> AddAsync(Order order);
		Task<int> CountByCustomerSinceAsync(int customerId, DateTime since);
		Task<bool> CustomerHasOrdersAsync(int customerId);
	}
}
