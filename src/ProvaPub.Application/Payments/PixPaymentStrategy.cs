using ProvaPub.Domain;

namespace ProvaPub.Application.Payments
{
	public class PixPaymentStrategy : IPaymentStrategy
	{
		public string PaymentMethod => "pix";

		public Task ProcessAsync(Order order)
		{
			//Faz pagamento via Pix...
			return Task.CompletedTask;
		}
	}
}
