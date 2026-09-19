using ProvaPub.Domain;

namespace ProvaPub.Application.Payments
{
	public class CreditCardPaymentStrategy : IPaymentStrategy
	{
		public string PaymentMethod => "creditcard";

		public Task ProcessAsync(Order order)
		{
			//Faz pagamento via cartão de crédito...
			return Task.CompletedTask;
		}
	}
}
