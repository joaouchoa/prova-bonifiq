using ProvaPub.Domain;

namespace ProvaPub.Application.Payments
{
	public interface IPaymentStrategy
	{
		string PaymentMethod { get; }

		Task ProcessAsync(Order order);
	}
}
