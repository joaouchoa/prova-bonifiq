using ProvaPub.Domain;

namespace ProvaPub.Application.Payments
{
	public class PaypalPaymentStrategy : IPaymentStrategy
	{
		public string PaymentMethod => "paypal";

		public Task ProcessAsync(Order order)
		{
			//Faz pagamento via PayPal...
			return Task.CompletedTask;
		}
	}
}
